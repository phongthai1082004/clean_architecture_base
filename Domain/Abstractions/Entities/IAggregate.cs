using Domain.Abstractions.Events;

namespace Domain.Abstractions.Entities
{
    public interface IAggregate
    {
        IReadOnlyList<IDomainEvent> DomainEvents { get; }
        void AddDomainEvent(IDomainEvent domainEvent);
        IDomainEvent[] ClearDomainEvents();
    }
}
