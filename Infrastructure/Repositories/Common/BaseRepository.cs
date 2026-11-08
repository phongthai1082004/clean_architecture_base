using Domain.Abstractions.Entities;
using Domain.Abstractions.Repository.Common;
using Domain.Abstractions.Shared;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories.Common
{
    public class BaseRepository<T, TKey>(
        ApplicationDbContext context,
        ILoggerFactory loggerFactory) : IBaseRepository<T, TKey> where T : BaseEntity<TKey>
    {
        protected readonly DbSet<T> dbSet = context.Set<T>();
        protected readonly ILogger logger = loggerFactory.CreateLogger($"Repository.{typeof(T).Name}");

        // Queries
        public async Task<T?> GetByIdAsync(object[] keyValues, CancellationToken cancellationToken = default)
        {
            var entity = await dbSet.FindAsync(keyValues, cancellationToken);
            logger.LogDebug("GetById {Entity} found={Found}", typeof(T).Name, entity is not null);
            return entity;
        }

        public async Task<T?> GetByIdNoTrackingAsync(object[] keyValues, CancellationToken cancellationToken = default)
        {
            var entity = await dbSet.FindAsync(keyValues, cancellationToken);

            if (entity != null)
            {
                context.Entry(entity).State = EntityState.Detached;
            }
            return entity;
        }

        public async Task<bool> CheckExist(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            if (spec.Criteria == null) throw new ArgumentException("Missing Criteria!");
            return await dbSet.AnyAsync(spec.Criteria, cancellationToken);
        }

        public async Task<int> CountAsync(ISpecification<T>? spec = null, CancellationToken cancellationToken = default)
        {
            if (spec?.Criteria == null)
            {
                return await dbSet.CountAsync(cancellationToken);
            }

            return await dbSet.CountAsync(spec.Criteria, cancellationToken);
        }

        public async Task<PagedResultBase<T>> GetListAsync(
            PageRequest request,
            ISpecification<T>? specification = null,
            Func<T, (DateTimeOffset? CursorTime, string? CursorId)>? cursorSelector = null,
            CancellationToken cancellationToken = default)
        {
            var query = SpecificationEvaluator.GetQuery(dbSet, specification);
            var result = await ApplyPaginationAsync(query, request, cursorSelector, cancellationToken);
            logger.LogDebug(
                "List {Entity} cursor={IsCursor} page={Page} size={PageSize}",
                typeof(T).Name, request.IsCursor, request.CurrentPage, result.PageSize);
            return result;
        }

        public async Task<PagedResultBase<TResult>> GetListAsync<TResult>(
            PageRequest request,
            ISpecification<T, TResult> specification,
            Func<TResult, (DateTimeOffset? CursorTime, string? CursorId)>? cursorSelector = null,
            CancellationToken cancellationToken = default) where TResult : class, ICursorItem<TKey>
        {
            var projectedQuery = SpecificationEvaluator.GetQuery(dbSet, specification);
            var result = await ApplyPaginationAsync(projectedQuery, request, cursorSelector, cancellationToken);
            logger.LogDebug(
                "List {Entity}->{Result} cursor={IsCursor} page={Page} size={PageSize}",
                typeof(T).Name, typeof(TResult).Name, request.IsCursor, request.CurrentPage, result.PageSize);
            return result;
        }

        private static async Task<PagedResultBase<TItem>> ApplyPaginationAsync<TItem>(
            IQueryable<TItem> query,
            PageRequest request,
            Func<TItem, (DateTimeOffset? CursorTime, string? CursorId)>? cursorSelector,
            CancellationToken cancellationToken = default) where TItem : class, ICursorItem<TKey>
        {
            const int maxPageSize = 100;
            var pageSize = request.PageSize <= 0 ? 10 : Math.Min(request.PageSize, maxPageSize);

            if (request.IsCursor)
            {
                if (!string.IsNullOrWhiteSpace(request.CursorId))
                {
                    if (typeof(TKey) == typeof(Guid) && Guid.TryParse(request.CursorId, out var guidCursorId))
                    {
                        query = request.CursorTime is { } cursorTime
                            ? query.Where(item => item.CreatedOnUtc < cursorTime
                                || (item.CreatedOnUtc == cursorTime && (Guid)(object)item.Id! < guidCursorId))
                            : query.Where(item => (Guid)(object)item.Id! < guidCursorId);
                    }
                    else if (typeof(TKey) == typeof(int) && int.TryParse(request.CursorId, out var intCursorId))
                    {
                        query = request.CursorTime is { } cursorTime
                            ? query.Where(item => item.CreatedOnUtc < cursorTime
                                || (item.CreatedOnUtc == cursorTime && (int)(object)item.Id! < intCursorId))
                            : query.Where(item => (int)(object)item.Id! < intCursorId);
                    }
                    else if (typeof(TKey) == typeof(long) && long.TryParse(request.CursorId, out var longCursorId))
                    {
                        query = request.CursorTime is { } cursorTime
                            ? query.Where(item => item.CreatedOnUtc < cursorTime
                                || (item.CreatedOnUtc == cursorTime && (long)(object)item.Id! < longCursorId))
                            : query.Where(item => (long)(object)item.Id! < longCursorId);
                    }
                }

                query = query.OrderByDescending(item => item.CreatedOnUtc).ThenByDescending(item => item.Id);

                var result = await query.Take(pageSize + 1).ToListAsync(cancellationToken);

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

            var count = await query.CountAsync(cancellationToken);
            var pageCount = (int)Math.Ceiling(count / (double)pageSize);
            var currentPage = Math.Max(request.CurrentPage ?? 1, 1);

            var pagedList = await query
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

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
            logger.LogDebug("Insert {Entity}", typeof(T).Name);
            dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            logger.LogDebug("Update {Entity}", typeof(T).Name);
            dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            logger.LogDebug("Delete {Entity}", typeof(T).Name);
            dbSet.Remove(entity);
        }
        public async Task DeleteByIdAsync(TKey id, CancellationToken cancellationToken = default)
        {
            var existingEntity = await dbSet.FindAsync([id], cancellationToken) ?? throw new KeyNotFoundException($"{typeof(T).Name} with ID '{id}' was not found.");
            logger.LogDebug("DeleteById {Entity} id={Id}", typeof(T).Name, id);
            dbSet.Remove(existingEntity);
        }

        // Bulk Operations
        public void BulkInsert(IEnumerable<T> entities)
        {
            logger.LogDebug("BulkInsert {Entity}", typeof(T).Name);
            dbSet.AddRange(entities);
        }

        public void BulkUpdate(IEnumerable<T> entities)
        {
            logger.LogDebug("BulkUpdate {Entity}", typeof(T).Name);
            dbSet.UpdateRange(entities);
        }

        public void BulkDelete(IEnumerable<T> entities)
        {
            logger.LogDebug("BulkDelete {Entity}", typeof(T).Name);
            dbSet.RemoveRange(entities);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            var affected = await context.SaveChangesAsync(cancellationToken);
            logger.LogDebug("SaveChanges {Entity} affected={Affected}", typeof(T).Name, affected);
        }
    }
}
