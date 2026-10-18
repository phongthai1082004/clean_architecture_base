using Domain.Abstractions.Entities;

namespace Domain.Entities.Users
{
    public class ExternalToken(
        Guid userId,
        string provider,
        string providerUserId,
        string accessToken,
        string? refreshToken,
        DateTime? expiresAt) : BaseEntity<int>
    {
        public Guid UserId { get; private set; } = userId;
        public string Provider { get; private set; } = provider;
        public string ProviderUserId { get; private set; } = providerUserId;
        public string AccessToken { get; private set; } = accessToken;
        public string? RefreshToken { get; private set; } = refreshToken;
        public DateTime? ExpiresAt { get; private set; } = expiresAt;

        public void UpdateTokens(string accessToken, string? refreshToken, DateTime? expiresAt)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            ExpiresAt = expiresAt;
        }

        public bool IsExpired() => ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;
    }
}
