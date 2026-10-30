using Domain.Abstractions.Repository;
using Domain.Abstractions.Shared;

namespace Infrastructure.Persistence.Repositories.Common
{
    public class BaseRepository<T> : IBaseRepository<T>
    {
        // Queries
        public Task<T?> GetByIdAsync<TKey>(TKey id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<T>> GetListAsync(PagedRequest pagedRequest, ISpecification<T>? spec = null, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> BulkInsertAsync(IReadOnlyList<T> entities, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> BulkUpdateAsync(IReadOnlyList<T> entities, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckExist<TKey>(TKey id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(ISpecification<T>? spec = null, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteByIdAsync<TKey>(TKey id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteByIdsAsync<TKey>(IReadOnlyList<TKey> ids, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        

        public Task<T> InsertAsync(T entity, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
