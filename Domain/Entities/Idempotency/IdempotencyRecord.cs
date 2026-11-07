using Domain.Abstractions.Entities;
using Domain.Enums.Idempotency;
using System.Security.Cryptography;
using System.Text;

namespace Domain.Entities.Idempotency
{
    public class IdempotencyRecord(
        string key,
        Guid? userId,
        string endpoint,
        string requestHash,
        DateTime expiresAt) : BaseEntity<int>
    {
        public string Key { get; private set; } = key;
        public Guid? UserId { get; private set; } = userId;
        public string Endpoint { get; private set; } = endpoint;
        public string? RequestHash { get; private set; } = requestHash;
        public IdempotencyStatus Status { get; private set; } = IdempotencyStatus.Pending;
        public int? ResponseStatusCode { get; private set; }
        public string? ResponseBody { get; private set; }
        public string? ResponseContentType { get; private set; }
        public DateTime ExpiresAt { get; private set; } = expiresAt;

        public void Complete(int statusCode, string? body, string? contentType)
        {
            Status = IdempotencyStatus.Completed;
            ResponseStatusCode = statusCode;
            ResponseBody = body;
            ResponseContentType = contentType;
            ExpiresAt = DateTime.UtcNow.AddDays(1);
        }

        public bool IsExpired() => ExpiresAt <= DateTime.UtcNow;
        public void MarkFailed() => Status = IdempotencyStatus.Failed;
        /// <summary>
        /// Computes a SHA-256 hash using the provided endpoint, userId, and request payload.
        /// </summary>
        public static string ComputeRequestHash(string endpoint, Guid? userId, string? requestBody)
        {
            string rawPayload = string.Join("|",
                endpoint,
                userId?.ToString() ?? string.Empty,
                requestBody ?? string.Empty
            );

            byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawPayload));
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }
    }
}
