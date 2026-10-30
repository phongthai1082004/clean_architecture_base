
namespace Domain.Abstractions.Auditable
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        DateTimeOffset? DeleteOnUtc { get; set; }
        Guid? DeletedBy { get; set; }
    }
}
