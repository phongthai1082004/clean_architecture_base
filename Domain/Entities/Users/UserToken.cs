using Domain.Abstractions.Entities;
using Domain.Enums.Users;

namespace Domain.Entities.Users
{
    public class UserToken(
        Guid userId,
        string tokenValue,
        TokenType type,
        string deviceInfo,
        string ipAddress,
        DateTime expiresAt) : BaseEntity<int>
    {
        public Guid UserId { get; private set; } = userId;
        public string TokenValue { get; private set; } = tokenValue;
        public TokenType Type { get; private set; } = type;
        public string DeviceInfo { get; private set; } = deviceInfo;
        public string IpAddress { get; private set; } = ipAddress;
        public bool IsRevoked { get; private set; } = false;
        public DateTime ExpiresAt { get; private set; } = expiresAt;
    }
}
