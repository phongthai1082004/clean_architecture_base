using Domain.Abstractions.Shared;

namespace Domain.Abstractions.Repository
{
    public interface IBaseRepository<T>
    {
        // Queries
        Task<T?> GetByIdAsync<TKey>(TKey id, CancellationToken ct = default);
        Task<bool> CheckExist<TKey>(TKey id, CancellationToken ct = default);
        Task<int> CountAsync(ISpecification<T>? spec = null, CancellationToken ct = default);
        Task<PagedResult<T>> GetListAsync(
            PagedRequest pagedRequest,
            ISpecification<T>? spec = null,
            CancellationToken ct = default);

        // Commands
        Task<T> InsertAsync(T entity, CancellationToken ct = default);
        Task<int> BulkInsertAsync(IReadOnlyList<T> entities, CancellationToken ct = default);
        Task UpdateAsync(T entity, CancellationToken ct = default);
        Task<int> BulkUpdateAsync(IReadOnlyList<T> entities, CancellationToken ct = default);
        Task DeleteByIdAsync<TKey>(TKey id, CancellationToken ct = default);
        Task<int> DeleteByIdsAsync<TKey>(IReadOnlyList<TKey> ids, CancellationToken ct = default);
    }
}
