namespace Domain.Abstractions.Events
{
    /// <summary>Something that happened in the domain, used for in-process side effects.</summary>
    public interface IDomainEvent
    {
        /// <summary>Unique event id (deduplication and tracing).</summary>
        Guid EventId { get; }

        /// <summary>UTC time the event was raised.</summary>
        DateTimeOffset OccurredOn { get; }
    }
}
