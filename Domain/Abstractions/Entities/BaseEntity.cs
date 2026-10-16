using Domain.Abstractions.Auditable;

namespace Domain.Abstractions.Entities
{
    public abstract class BaseEntity<TKey> : IAuditable 
    {
        public TKey Id { get; protected set; } = default!;
        public DateTimeOffset CreatedOnUtc { get; set; }
        public Guid? CreatedBy { get ; set ; }
        public DateTimeOffset? LastModifiedUtc { get ; set ; }
        public Guid? LastModifiedBy { get ; set ; }
    }
}
