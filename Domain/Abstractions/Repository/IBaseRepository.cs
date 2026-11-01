using Domain.Abstractions.Shared;

namespace Domain.Abstractions.Repository
{
    public interface IBaseRepository<T, TKey>
    {
        // Queries
        Task<T?> GetByIdAsync(object[] keyValues, CancellationToken ct = default);
        Task<bool> CheckExist(ISpecification<T> spec, CancellationToken ct = default);
        Task<int> CountAsync(ISpecification<T>? spec = null, CancellationToken ct = default);
        Task<PagedResult<T>> GetListAsync(
            PagedRequest pagedRequest,
            ISpecification<T>? spec = null,
            CancellationToken ct = default);

        // Single Commands
        void Insert(T entity);
        void Update(T entity); 
        void Delete(T entity);
        Task DeleteByIdAsync(TKey id, CancellationToken ct = default);

        // Bulk Operations
        void BulkInsert(IEnumerable<T> entities);
        void BulkUpdate(IEnumerable<T> entities);
        void BulkDelete(IEnumerable<T> entities);

        // Streaming Commands
        Task<int> BulkInsertStreamingAsync(
            IAsyncEnumerable<T> entities,
            int batchSize = 1000,
            CancellationToken ct = default);
        Task<int> BulkUpdateStreamingAsync(
            IAsyncEnumerable<T> entities, 
            int batchSize = 1000,
            CancellationToken ct = default);
        Task<int> DeleteByIdsStreamingAsync(
            IAsyncEnumerable<TKey> ids, 
            int batchSize = 1000,
            CancellationToken ct = default);
    }
}
