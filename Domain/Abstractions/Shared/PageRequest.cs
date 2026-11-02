namespace Domain.Abstractions.Shared
{
    public abstract record PageRequest
    {
        public int PageSize { get; init; } = 20;
        public int? CurrentPage { get; init; }
        public DateTimeOffset? CursorTime { get; init; }
        public string? CursorId { get; init; }
        public bool IsCursor => CursorTime.HasValue || !string.IsNullOrWhiteSpace(CursorId);
    }
}
