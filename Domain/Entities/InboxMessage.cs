using Domain.Abstractions.Entities;

namespace Domain.Entities
{
    public class InboxMessage : GuidEntity
    {
        public DateTime ProcessedAt { get; private set; }
        public string? Error { get; private set; }

        public InboxMessage(Guid id)
        {
            Id = id;
            ProcessedAt = DateTime.UtcNow;
        }

        public void MarkAsFailed(string errorMessage)
        {
            Error = errorMessage;
        }
    }
}
