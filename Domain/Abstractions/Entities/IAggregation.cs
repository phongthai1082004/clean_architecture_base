using Domain.Abstractions.Auditable;

namespace Domain.Abstractions.Entities
{
    public interface IAggregate : IAuditable, ISoftDeletable
    {
        IReadOnlyList<IDomainEvent> DomainEvents { get; }
        void AddDomainEvent(IDomainEvent domainEvent);
        IDomainEvent[] ClearDomainEvents();
    }
}
