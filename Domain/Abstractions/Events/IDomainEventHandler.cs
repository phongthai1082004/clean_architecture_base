namespace Domain.Abstractions.Events
{
    public interface IDomainEventHandler<TEvent> where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent domainEvent, CancellationToken ct = default);
    }
}
