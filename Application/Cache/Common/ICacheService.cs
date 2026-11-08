using Domain.Enums;

namespace Application.Cache.Common
{
    /// <summary>Key/value cache with optional conditional writes (Redis implementation).</summary>
    public interface ICacheService
    {
        /// <summary>Reads and deserializes the value for <paramref name="key"/>; returns <c>default</c> on a miss.</summary>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Stores <paramref name="value"/>; <paramref name="condition"/> selects always / only if missing
        /// (used as a distributed lock) / only if present. Returns <c>true</c> when the write succeeded.
        /// </summary>
        Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null, CacheCondition? condition = CacheCondition.Always);

        /// <summary>Deletes <paramref name="key"/>; returns <c>true</c> when a key was removed.</summary>
        Task<bool> RemoveAsync(string key);

        /// <summary>Runs a Lua script atomically against <paramref name="keys"/> (used to release the idempotency lock).</summary>
        Task<bool> ExecuteScriptAsync(string script, string[] keys, object[]? args = null);
    }
}
