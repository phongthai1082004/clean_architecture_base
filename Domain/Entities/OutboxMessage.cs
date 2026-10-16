using Domain.Abstractions.Entities;
namespace Domain.Entities
{
    public class OutboxMessage : GuidEntity
    {
        public string EventType { get; private set; }
        public string Payload { get; private set; } 
        public DateTime OccurredOn { get; private set; }
        public DateTime? ProcessedAt { get; private set; }
        public string? Error { get; private set; }

        internal OutboxMessage(string eventType, string payload, DateTime occurredOn)
        {
            EventType = eventType;
            Payload = payload;
            OccurredOn = occurredOn;
            ProcessedAt = null;
        }

        public void MarkAsProcessed()
        {
            ProcessedAt = DateTime.UtcNow;
            Error = null;
        }

        public void MarkAsFailed(string errorMessage)
        {
            Error = errorMessage;
        }
    }
}
