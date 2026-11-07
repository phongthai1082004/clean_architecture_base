using Domain.Abstractions.Repository.Common;

namespace Domain.Abstractions.Repository
{
    public interface IIdempotentRepository<TEntity, TKey>
       : IBaseRepository<TEntity, TKey>
    {
        Task<TEntity?> GetByIdempotencyKey(string idempotencyKey, Guid? userId, string endpoint, CancellationToken cancellationToken);
    }
}
