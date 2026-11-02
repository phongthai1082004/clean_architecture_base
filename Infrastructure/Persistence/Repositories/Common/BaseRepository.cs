using Domain.Abstractions.Entities;
using Domain.Abstractions.Repository;
using Domain.Abstractions.Shared;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Common
{
    public class BaseRepository<T, TKey>(
        ApplicationDbContext context) : IBaseRepository<T, TKey> where T : BaseEntity<TKey>
    {
        protected readonly DbSet<T> dbSet = context.Set<T>();

        // Queries
        public async Task<T?> GetByIdAsync(object[] keyValues, CancellationToken ct = default)
        {
            return await dbSet.FindAsync(keyValues, ct);
        }

        public async Task<T?> GetByIdNoTrackingAsync(object[] keyValues, CancellationToken ct = default)
        {
            var entity = await dbSet.FindAsync(keyValues, ct);

            if (entity != null)
            {
                context.Entry(entity).State = EntityState.Detached;
            }
            return entity;
        }

        public async Task<bool> CheckExist(ISpecification<T> spec, CancellationToken ct = default)
        {
            if (spec.Criteria == null) throw new ArgumentException("Missing Criteria!");
            return await dbSet.AnyAsync(spec.Criteria, ct);
        }

        public async Task<int> CountAsync(ISpecification<T>? spec = null, CancellationToken ct = default)
        {
            if (spec?.Criteria == null)
            {
                return await dbSet.CountAsync(ct);
            }

            return await dbSet.CountAsync(spec.Criteria, ct);
        }

        public async Task<PagedResultBase<T>> GetListAsync(PageRequest request, ISpecification<T>? specification = null, Func<T, (DateTimeOffset? CursorTime, string? CursorId)>? cursorSelector = null, CancellationToken ct = default)
        {
            var query = dbSet.AsQueryable();

            query = SpecificationEvaluator.GetQuery<T>(query, specification);

        }

        public Task<PagedResultBase<TResult>> GetListAsync<TResult>(PageRequest request, ISpecification<T, TResult> specification, Func<TResult, (DateTimeOffset? CursorTime, string? CursorId)>? cursorSelector = null, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        // Commands
        public void Insert(T entity)
        {
            dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public async Task DeleteByIdAsync(TKey id, CancellationToken ct = default)
        {
            var existingEntity = await dbSet.FindAsync([id], ct) ?? throw new KeyNotFoundException($"{typeof(T).Name} with ID '{id}' was not found.");
            dbSet.Remove(existingEntity);
        }

        // Bulk Operations
        public void BulkInsert(IEnumerable<T> entities)
        {
            dbSet.AddRange(entities);
        }

        public void BulkUpdate(IEnumerable<T> entities)
        {
            dbSet.UpdateRange(entities);
        }

        public void BulkDelete(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

        // Bulk Streaming
        public async Task<int> BulkInsertStreamingAsync(IAsyncEnumerable<T> dataStream, int batchSize = 1000, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
        
        public Task<int> BulkUpdateStreamingAsync(IAsyncEnumerable<T> entities, int batchSize = 1000, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
        
        public Task<int> DeleteByIdsStreamingAsync(IAsyncEnumerable<TKey> ids, int batchSize = 1000, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
        
        private async Task ProcessBatchAsync(List<T> batch, CancellationToken ct)
        {
            context.Set<T>().AddRange(batch);
            await context.SaveChangesAsync(ct);
            context.ChangeTracker.Clear();
        }
    }
}
