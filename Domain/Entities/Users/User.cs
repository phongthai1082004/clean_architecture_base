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
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }

        private readonly List<UserToken> _tokens = new();
        private readonly List<ExternalToken> _externalTokens = new();
        public IReadOnlyCollection<UserToken> Tokens => _tokens.AsReadOnly();
        public IReadOnlyCollection<ExternalToken> ExternalTokens => _externalTokens.AsReadOnly();
    }
}
