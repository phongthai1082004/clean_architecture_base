using Domain.Abstractions.Events;

namespace Domain.Events
{
    public record UserRegisteredEvent(Guid UserId, string Email, string FullName) : DomainEvent;

    public record UserActivatedEvent(Guid UserId) : DomainEvent;

    public record UserBannedEvent(Guid UserId) : DomainEvent;

    public record UserProfileUpdatedEvent(Guid UserId, string FullName, string? PhoneNumber) : DomainEvent;

    public record UserPasswordChangedEvent(Guid UserId) : DomainEvent;

    public record UserEmailChangedEvent(Guid UserId, string NewEmail) : DomainEvent;
}
