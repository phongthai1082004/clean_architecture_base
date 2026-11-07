namespace Application.Cache.Common
{
    public static class CacheKeys
    {
        public static string IdempotencyLockData<TCommand>(string key, Guid? userId, string endpoint)
            => $"idempotency:lock:{typeof(TCommand).Name.ToLowerInvariant()}:{Scope(userId, endpoint)}:{key.Trim()}";

        public static string IdempotencyShortData<TCommand>(string key, Guid? userId, string endpoint)
            => $"idempotency:data:{typeof(TCommand).Name.ToLowerInvariant()}:{Scope(userId, endpoint)}:{key.Trim()}";

        private static string Scope(Guid? userId, string endpoint) =>
            $"{userId?.ToString() ?? "anon"}:{endpoint}";
    }
}
