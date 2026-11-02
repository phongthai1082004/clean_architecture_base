
namespace Domain.Abstractions.Auditable
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        DateTimeOffset? DeletedOnUtc { get; set; }
        Guid? DeletedBy { get; set; }
    }
}
