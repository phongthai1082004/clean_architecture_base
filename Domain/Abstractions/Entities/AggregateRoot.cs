namespace Domain.Abstractions.Entities
{
    public class AggregateRoot : GuidEntity, IAggregate
    {
        private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
        public bool IsDeleted { get ; set ; }
        public DateTimeOffset? DeleteOnUtc { get ; set ; }
        public Guid? DeletedBy { get ; set ; }
        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }
        public IDomainEvent[] ClearDomainEvents()
        {
            IDomainEvent[] dequeuedEvents = _domainEvents.ToArray();
            _domainEvents.Clear();
            return dequeuedEvents;
        }
    }
}
