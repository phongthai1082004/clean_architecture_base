using Domain.Abstractions.Auditable;

namespace Domain.Abstractions.Entities
{
    public class BaseEntitySoftDelete<TKey> : BaseEntity<TKey>, ISoftDelete
    {
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeleteOnUtc { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}
