namespace Domain.Abstractions.Shared
{
    public abstract class PagedResultBase<T>
    {
        public IReadOnlyList<T> Items { get; set; } = [];
        public int PageSize { get; set; }
    }
}
