namespace Application.Cache.Common
{
    public static class CacheKeys
    {
        public static string IdempotencyLockData<TCommand>(string key) 
            => $"idempotency:lock:{typeof(TCommand).Name.ToLowerInvariant()}:{key.Trim()}";
        public static string IdempotencyShortData<TCommand>(string key) 
            => $"idempotency:data:{typeof(TCommand).Name.ToLowerInvariant()}:{key.Trim()}";

    }
}
