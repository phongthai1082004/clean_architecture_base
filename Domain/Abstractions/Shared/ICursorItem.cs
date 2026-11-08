namespace Domain.Abstractions.Shared
{
    /// <summary>Contract required by cursor (keyset) pagination.</summary>
    /// <typeparam name="TKey">Primary key type.</typeparam>
    public interface ICursorItem<TKey>
    {
        /// <summary>Primary key, used as the cursor tie-breaker.</summary>
        TKey Id { get; }

        /// <summary>Creation timestamp, used as the primary cursor sort column.</summary>
        DateTimeOffset CreatedOnUtc { get; }
    }
}
