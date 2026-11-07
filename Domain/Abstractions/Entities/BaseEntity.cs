
using Domain.Abstractions.Auditable;
using Domain.Abstractions.Shared;

namespace Domain.Abstractions.Entities
{
    public abstract class BaseEntity<TKey> : IAuditable, ICursorItem<TKey>
    {
        public TKey Id { get; protected set; } = default!;
        public DateTimeOffset CreatedOnUtc { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTimeOffset? LastModifiedOnUtc { get; set; }
        public Guid? LastModifiedBy { get; set; }
        public BaseEntity()
        {

        }
    }
}
