using Application.Cache.Common;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Cache
{
    public class CacheService(IDatabase redis, ILogger<CacheService> logger) : ICacheService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public async Task<T?> GetAsync<T>(string key)
        {
            logger.LogDebug("Cache get {Key}", key);

            RedisValue value = await redis.StringGetAsync(key);
            if (value.IsNullOrEmpty)
            {
                logger.LogDebug("Cache miss {Key}", key);
                return default;
            }

            return JsonSerializer.Deserialize<T>(value.ToString(), JsonOptions);
        }

        public async Task<bool> RemoveAsync(string key)
        {
            logger.LogDebug("Cache remove {Key}", key);
            return await redis.KeyDeleteAsync(key);
        }

        public async Task<bool> SetAsync<T>(
            string key,
            T value,
            TimeSpan? expiry = null,
            CacheCondition? condition = CacheCondition.Always)
        {
            logger.LogDebug("Cache set {Key} condition={Condition} expiry={Expiry}", key, condition, expiry);

            var json = JsonSerializer.Serialize(value, JsonOptions);

            var redisWhen = condition switch
            {
                CacheCondition.NotExists => When.NotExists,
                CacheCondition.Exists => When.Exists,
                _ => When.Always
            };

            return await redis.StringSetAsync(key, json, expiry, redisWhen);
        }

        public async Task<bool> ExecuteScriptAsync(string script, string[] keys, object[]? args = null)
        {
            logger.LogDebug("Cache script execute on {Keys}", string.Join(",", keys));

            var redisKeys = Array.ConvertAll(keys, k => (RedisKey)k);
            var redisArgs = args is not null
                ? Array.ConvertAll(args, a => (RedisValue)a.ToString()!)
                : [];

            var result = await redis.ScriptEvaluateAsync(script, redisKeys, redisArgs);

            if (result.IsNull)
            {
                return false;
            }

            try
            {
                return (long)result == 1;
            }
            catch (InvalidCastException)
            {
                return (bool)result;
            }
        }
    }
}
