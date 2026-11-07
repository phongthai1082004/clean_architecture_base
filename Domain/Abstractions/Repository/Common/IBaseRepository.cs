using Domain.Abstractions.Shared;

namespace Domain.Abstractions.Repository.Common
{
    public interface IBaseRepository<T, TKey>
    {
        // Queries
        Task<T?> GetByIdAsync(object[] keyValues, CancellationToken ct = default);
        Task<bool> CheckExist(ISpecification<T> spec, CancellationToken ct = default);
        Task<int> CountAsync(ISpecification<T>? spec = null, CancellationToken ct = default);
        Task<PagedResultBase<T>> GetListAsync(
            PageRequest request,
            ISpecification<T>? specification = null,
            Func<T, (DateTimeOffset? CursorTime, string? CursorId)>? cursorSelector = null,
            CancellationToken ct = default);

        Task<PagedResultBase<TResult>> GetListAsync<TResult>(
            PageRequest request,
            ISpecification<T, TResult> specification,
            Func<TResult, (DateTimeOffset? CursorTime, string? CursorId)>? cursorSelector = null,
            CancellationToken ct = default) where TResult : class, ICursorItem<TKey>;

        // Single Commands
        void Insert(T entity);
        void Update(T entity); 
        void Delete(T entity);
        Task DeleteByIdAsync(TKey id, CancellationToken ct = default);

        // Bulk Operations
        void BulkInsert(IEnumerable<T> entities);
        void BulkUpdate(IEnumerable<T> entities);
        void BulkDelete(IEnumerable<T> entities);

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
