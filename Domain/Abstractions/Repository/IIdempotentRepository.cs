using Domain.Abstractions.Repository.Common;

namespace Domain.Abstractions.Repository
{
    /// <summary>Repository for stored idempotency records.</summary>
    /// <typeparam name="TEntity">Idempotency record type.</typeparam>
    /// <typeparam name="TKey">Primary key type.</typeparam>
    public interface IIdempotentRepository<TEntity, TKey>
       : IBaseRepository<TEntity, TKey>
    {
        /// <summary>Finds a stored request by idempotency key, user id and endpoint.</summary>
        Task<TEntity?> GetByIdempotencyKey(string idempotencyKey, Guid? userId, string endpoint, CancellationToken cancellationToken);
    }
}
