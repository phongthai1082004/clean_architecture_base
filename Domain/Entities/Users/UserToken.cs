using Domain.Abstractions.Entities;
using Domain.Enums.Users;

namespace Domain.Entities.Users
{
    public class UserToken(
        Guid userId,
        string tokenValue,
        TokenType type,
        DateTimeOffset expiresAt) : BaseEntity<int>
    {
        public Guid UserId { get; private set; } = userId;
        public string TokenValue { get; private set; } = tokenValue;
        public TokenType Type { get; private set; } = type;
        public bool IsRevoked { get; private set; } = false;
        public DateTimeOffset ExpiresAt { get; private set; } = expiresAt;

        public void Revoke()
        {
            IsRevoked = true;
        }

        public bool IsExpired() => ExpiresAt <= DateTime.UtcNow;
        public bool IsValid() => !IsRevoked && !IsExpired();
    }
}
