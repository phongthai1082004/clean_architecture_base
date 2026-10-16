
namespace Domain.Abstractions.Auditable
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        DateTimeOffset? DeleteOnUtc { get; set; }
        Guid? DeletedBy { get; set; }
    }
}
