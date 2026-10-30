using Domain.Abstractions.Events;

namespace Domain.Abstractions.Entities
{
    public class AggregateRoot : GuidEntitySoftDelete, IAggregate
    {
        private readonly List<IDomainEvent> _domainEvents = [];
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public IDomainEvent[] ClearDomainEvents()
        {
            IDomainEvent[] dequeuedEvents = [.. _domainEvents];
            _domainEvents.Clear();
            return dequeuedEvents;
        }
    }
}
