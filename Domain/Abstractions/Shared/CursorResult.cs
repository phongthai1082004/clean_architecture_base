namespace Domain.Abstractions.Shared
{
    public class CursorResult<T> : PagedResultBase<T>
    {
        public bool HasNextPage { get; set; }
        public DateTimeOffset? LastCursorTime { get; set; }
        public string? LastCursorId { get; set; }
        public CursorResult(
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