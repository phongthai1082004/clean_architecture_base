using Domain.Enums.Idempotency;

namespace Application.Cache.Common
{
    public record IdempotencyCacheEntry(
        string RequestHash,
        IdempotencyStatus Status,
        string? ResponseBody);
}
