
using Domain.Abstractions.Auditable;
using Domain.Abstractions.Entities;
using Domain.Abstractions.Shared;

namespace Infrastructure.Utils
{
    public static class PagedListExtensions
    {
        public static async Task<PagedResultBase<T>> ToPageResult<T>(
            this IQueryable<T> query,
            PageRequest request, 
            CancellationToken ct) where T : IAuditable
        {
            return request.IsCursor ?
                await ToCursorResultAsync(query, request, ct) :
                await ToPaginatedResultAsync(query, request, ct);
        }

        private static async Task<CursorResult<T>> ToCursorResultAsync<T>(
            this IQueryable<T> query,
            PageRequest request,
            CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        private static async Task<PaginatedResult<T>> ToPaginatedResultAsync<T>(
            this IQueryable<T> query,
            PageRequest request,
            CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
