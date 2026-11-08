using Domain.Abstractions.Events;

namespace Domain.Abstractions.Entities
{
    /// <summary>Aggregate root that buffers domain events until they are dispatched.</summary>
    public interface IAggregate
    {
        /// <summary>Events raised by this aggregate that have not been dispatched yet.</summary>
        IReadOnlyList<IDomainEvent> DomainEvents { get; }

        /// <summary>Queues a new domain event.</summary>
        void AddDomainEvent(IDomainEvent domainEvent);

        /// <summary>Removes and returns all queued events (called after persistence).</summary>
        IDomainEvent[] ClearDomainEvents();
    }
}
