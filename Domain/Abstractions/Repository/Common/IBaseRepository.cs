using Domain.Abstractions.Shared;

namespace Domain.Abstractions.Repository.Common
{
    /// <summary>Generic persistence contract for an entity type.</summary>
    /// <typeparam name="T">Entity type.</typeparam>
    /// <typeparam name="TKey">Primary key type.</typeparam>
    public interface IBaseRepository<T, TKey>
    {
        // Queries

        /// <summary>Finds an entity by its primary key values; returns <c>null</c> when not found.</summary>
        Task<T?> GetByIdAsync(object[] keyValues, CancellationToken cancellationToken = default);

        /// <summary>Returns <c>true</c> when any entity matches the specification criteria.</summary>
        Task<bool> CheckExist(ISpecification<T> spec, CancellationToken cancellationToken = default);

        /// <summary>Counts all entities, or only those matching the specification criteria.</summary>
        Task<int> CountAsync(ISpecification<T>? spec = null, CancellationToken cancellationToken = default);

        /// <summary>Returns one page of entities using offset paging or cursor (keyset) paging.</summary>
        Task<PagedResultBase<T>> GetListAsync(
            PageRequest request,
            ISpecification<T>? specification = null,
            Func<T, (DateTimeOffset? CursorTime, string? CursorId)>? cursorSelector = null,
            CancellationToken cancellationToken = default);

        /// <summary>Returns one page of projected results (see <see cref="ISpecification{T,TResult}.Selector"/>).</summary>
        Task<PagedResultBase<TResult>> GetListAsync<TResult>(
            PageRequest request,
            ISpecification<T, TResult> specification,
            Func<TResult, (DateTimeOffset? CursorTime, string? CursorId)>? cursorSelector = null,
            CancellationToken cancellationToken = default) where TResult : class, ICursorItem<TKey>;

        // Single Commands

        /// <summary>Stages a new entity for insertion.</summary>
        void Insert(T entity);

        /// <summary>Marks an existing entity as modified.</summary>
        void Update(T entity);

        /// <summary>Stages an entity for deletion (soft delete when the entity supports it).</summary>
        void Delete(T entity);

        /// <summary>Loads the entity by key and stages it for deletion; throws when not found.</summary>
        Task DeleteByIdAsync(TKey id, CancellationToken cancellationToken = default);

        // Bulk Operations

        /// <summary>Stages many new entities for insertion.</summary>
        void BulkInsert(IEnumerable<T> entities);

        /// <summary>Marks many existing entities as modified.</summary>
        void BulkUpdate(IEnumerable<T> entities);

        /// <summary>Stages many entities for deletion.</summary>
        void BulkDelete(IEnumerable<T> entities);

        /// <summary>Flushes all staged changes to the database.</summary>
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
