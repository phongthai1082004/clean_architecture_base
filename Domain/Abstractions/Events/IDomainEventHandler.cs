namespace Domain.Abstractions.Events
{
    /// <summary>Handles a specific domain event.</summary>
    /// <typeparam name="TEvent">Event type to handle.</typeparam>
    public interface IDomainEventHandler<TEvent> where TEvent : IDomainEvent
    {
        /// <summary>Reacts to <paramref name="domainEvent"/>.</summary>
        Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
    }
}
