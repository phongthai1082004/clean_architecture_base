namespace Domain.Abstractions.Shared
{
    public class PagedResult
    {
        public int PageSize { get; set; }
        public bool HasNextPage { get; set; }
        public DateTimeOffset? LastCursorTime { get; set; }
        public string? LastCursorId { get; set; }
    }

    public class PagedResult<T> : PagedResult
    {
        public IReadOnlyList<T> Items { get; set; } = [];

        public PagedResult(
            IReadOnlyList<T> items,
            int pageSize,
            bool hasNextPage,
            DateTimeOffset? lastCursorTime = null,
            string? lastCursorId = null)
        {
            Items = items;
            PageSize = pageSize;
            HasNextPage = hasNextPage;
            LastCursorTime = lastCursorTime;
            LastCursorId = lastCursorId;
        }
    }
}