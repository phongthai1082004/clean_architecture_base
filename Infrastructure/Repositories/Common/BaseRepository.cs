using Domain.Abstractions.Entities;
using Domain.Abstractions.Repository.Common;
using Domain.Abstractions.Shared;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Common
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

        public async Task<PagedResultBase<T>> GetListAsync(
            PageRequest request,
            ISpecification<T>? specification = null,
            Func<T, (DateTimeOffset? CursorTime, string? CursorId)>? cursorSelector = null,
            CancellationToken ct = default)
        {
            var query = SpecificationEvaluator.GetQuery(dbSet, specification);
            return await ApplyPaginationAsync(query, request, cursorSelector, ct);
        }

        public async Task<PagedResultBase<TResult>> GetListAsync<TResult>(
            PageRequest request,
            ISpecification<T, TResult> specification,
            Func<TResult, (DateTimeOffset? CursorTime, string? CursorId)>? cursorSelector = null,
            CancellationToken ct = default) where TResult : class, ICursorItem<TKey>
        {
            var projectedQuery = SpecificationEvaluator.GetQuery(dbSet, specification);
            return await ApplyPaginationAsync(projectedQuery, request, cursorSelector, ct);
        }

        private static async Task<PagedResultBase<TItem>> ApplyPaginationAsync<TItem>(
            IQueryable<TItem> query,
            PageRequest request,
            Func<TItem, (DateTimeOffset? CursorTime, string? CursorId)>? cursorSelector,
            CancellationToken ct = default) where TItem : class, ICursorItem<TKey>
        {
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

            if (request.IsCursor)
            {
                if (!string.IsNullOrWhiteSpace(request.CursorId))
                {
                    if (Guid.TryParse(request.CursorId, out var guidCursorId) && request.CursorTime.HasValue)
                    {
                        var cursorTime = request.CursorTime.Value;
                        query = query.Where(item => item.CreatedOnUtc < cursorTime
                            || (item.CreatedOnUtc == cursorTime && (Guid)(object)item.Id! < guidCursorId));
                    }
                    else if (long.TryParse(request.CursorId, out var longCursorId))
                    {
                        if (typeof(TKey) == typeof(int))
                        {
                            var intCursorId = (int)longCursorId;
                            query = query.Where(item => (int)(object)item.Id! < intCursorId);
                        }
                        else
                        {
                            query = query.Where(item => (long)(object)item.Id! < longCursorId);
                        }
                    }
                }

                query = typeof(TKey) == typeof(Guid)
                    ? query.OrderByDescending(item => item.CreatedOnUtc).ThenByDescending(item => item.Id)
                    : query.OrderByDescending(item => item.Id);

                var result = await query.Take(pageSize + 1).ToListAsync(ct);

                var hasNextPage = result.Count > pageSize;
                if (hasNextPage)
                {
                    result.RemoveAt(pageSize);
                }

                var lastItem = result.Count > 0 ? result[^1] : null;
                DateTimeOffset? nextCursorTime = null;
                string? nextCursorId = null;

                if (lastItem != null)
                {
                    if (cursorSelector != null)
                    {
                        (nextCursorTime, nextCursorId) = cursorSelector(lastItem);
                    }
                    else
                    {
                        nextCursorTime = lastItem.CreatedOnUtc;
                        nextCursorId = lastItem.Id?.ToString();
                    }
                }

                return new CursorResult<TItem>(
                    result,
                    pageSize,
                    hasNextPage,
                    nextCursorTime,
                    nextCursorId);
            }

            var count = await query.CountAsync(ct);
            var pageCount = (int)Math.Ceiling(count / (double)pageSize);
            var currentPage = request.CurrentPage ?? 1;

            var pagedList = await query
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PaginatedResult<TItem>(
                pagedList,
                pageSize,
                currentPage,
                pageCount,
                count);
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
        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        private async Task ProcessBatchAsync(List<T> batch, CancellationToken ct)
        {
            context.Set<T>().AddRange(batch);
            await context.SaveChangesAsync(ct);
            context.ChangeTracker.Clear();
        }
    }
}
