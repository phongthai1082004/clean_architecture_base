namespace Domain.Abstractions.Shared
{
    public interface ICursorItem<TKey>
    {
        TKey Id { get; }
        DateTimeOffset CreatedOnUtc { get; }
    }
}
