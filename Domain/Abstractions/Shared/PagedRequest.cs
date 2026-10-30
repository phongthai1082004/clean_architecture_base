namespace Domain.Abstractions.Shared
{
    public class PagedRequest
    {
        public int PageSize { get; set; } = 20;
        public DateTimeOffset? LastCursorTime { get; set; }
        public string? LastCursorId { get; set; }
    }
}
