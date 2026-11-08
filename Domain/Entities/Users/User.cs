using Domain.Abstractions.Entities;
using Domain.Enums.Users;

namespace Domain.Entities.Users
{
    public class User(string email, string fullName, string? passwordHash, string? phoneNumber) : AggregateRoot
    {
        public string Email { get; private set; } = email;
        public string FullName { get; private set; } = fullName;
        public string? PasswordHash { get; private set; } = passwordHash;
        public string? PhoneNumber { get; private set; } = phoneNumber;
        public string AvatarUrl { get; private set; } = "";
        public UserStatus Status { get; private set; } = UserStatus.Unactivated;
        public UserRole Role { get; private set; } = UserRole.User;

        private readonly List<UserToken> _tokens = [];
        private readonly List<ExternalToken> _externalTokens = [];
        public IReadOnlyCollection<UserToken> Tokens => _tokens.AsReadOnly();
        public IReadOnlyCollection<ExternalToken> ExternalTokens => _externalTokens.AsReadOnly();

        public void Activate()
        {
            if (Status == UserStatus.Banned)
                throw new InvalidOperationException("Cannot activate a banned user.");

            Status = UserStatus.Active;
        }

        public void Ban()
        {
            Status = UserStatus.Banned;
            RevokeAllTokens();
        }

        public void Unban()
        {
            if (Status != UserStatus.Banned)
                return;
            Status = UserStatus.Active;
        }

        public void UpdateProfile(string fullName, string? phoneNumber, string avatarUrl)
        {
            FullName = fullName;
            PhoneNumber = phoneNumber;
            AvatarUrl = avatarUrl;
        }

        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
            RevokeAllTokens();
        }

        public void ChangeEmail(string newEmail)
        {
            Email = newEmail;
            Status = UserStatus.Unactivated;
        }

        public UserToken AddToken(string tokenValue, TokenType type, DateTimeOffset expiresAt)
        {
            var token = new UserToken(Id, tokenValue, type, expiresAt);
            _tokens.Add(token);
            return token;
        }

        public void RevokeToken(int tokenId)
        {
            var token = _tokens.FirstOrDefault(t => t.Id == tokenId);
            token?.Revoke();
        }

        public void RevokeAllTokens()
        {
            foreach (var token in _tokens.Where(t => !t.IsRevoked))
                token.Revoke();
        }

        public void AddExternalToken(ExternalProvider provider, string providerUserId, string accessToken, string? refreshToken, DateTime? expiresAt)
        {
            if (_externalTokens.Any(t => t.Provider == provider))
                throw new InvalidOperationException($"External token for provider '{provider}' already exists.");

            var token = new ExternalToken(Id, provider, providerUserId, accessToken, refreshToken, expiresAt);
            _externalTokens.Add(token);
        }

        public void RemoveExternalToken(ExternalProvider provider)
        {
            var token = _externalTokens.FirstOrDefault(t => t.Provider == provider);
            if (token != null)
                _externalTokens.Remove(token);
        }

        public bool IsTokenValid(int tokenId)
        {
            var token = _tokens.FirstOrDefault(t => t.Id == tokenId);
            return token != null && !token.IsRevoked && token.ExpiresAt > DateTime.UtcNow;
        }
    }
}
