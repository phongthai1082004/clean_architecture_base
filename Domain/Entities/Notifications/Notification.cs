using Domain.Abstractions.Entities;
using Domain.Enums.Notifications;

namespace Domain.Entities.Notifications
{
    public class Notification(
        Guid userId,
        string title,
        string message,
        NotificationType type) : GuidEntity
    {
        public Guid UserId { get; private set; } = userId;
        public string Title { get; private set; } = title;
        public string Message { get; private set; } = message;
        public NotificationType Type { get; private set; } = type;
        public bool IsRead { get; private set; } = false;

        public void MarkAsRead() => IsRead = true;

        public void MarkAsUnread() => IsRead = false;
    }
}
