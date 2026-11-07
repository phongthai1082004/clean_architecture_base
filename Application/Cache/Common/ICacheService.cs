using Domain.Enums;

namespace Application.Cache.Common
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null, CacheCondition? condition = CacheCondition.Always);
        Task<bool> RemoveAsync(string key);
        Task<bool> ExecuteScriptAsync(string script, string[] keys, object[]? args = null);
    }
}
