namespace Domain.Abstractions.Events
{
    public interface IDomainEvent
    {
        Guid EventId { get; }
        DateTimeOffset OccurredOn { get; }
    }
}
