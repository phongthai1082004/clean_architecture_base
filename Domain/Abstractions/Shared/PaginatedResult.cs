namespace Domain.Abstractions.Shared
{
    public class PaginatedResult<T> : PagedResultBase<T>
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int TotalCount { get; set; }
        public PaginatedResult(
            IReadOnlyList<T> items,
            int pageSize,
            int currentPage,
            int pageCount,
            int totalCount)
        {
            Items = items;
            PageSize = pageSize;
            CurrentPage = currentPage;
            PageCount = pageCount;
            TotalCount = totalCount;
        }
    }
}
