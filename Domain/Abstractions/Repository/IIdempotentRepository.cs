using Domain.Abstractions.Repository.Common;

namespace Domain.Abstractions.Repository
{
    public interface IIdempotentRepository<IdempotencyRecord, Guid>
       : IBaseRepository<IdempotencyRecord, Guid>
    {
        Task<bool> CheckExistByRequestHash(string requestHash, CancellationToken cancellation);
        Task<IdempotencyRecord?> GetByIdempotencyKey(string idempotencyKey, CancellationToken cancellationToken);
    }
}
