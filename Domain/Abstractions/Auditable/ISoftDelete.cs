
namespace Domain.Abstractions.Auditable
{
    /// <summary>Entity is logically deleted instead of physically removed.</summary>
    public interface ISoftDelete
    {
        /// <summary>True when the entity is deleted; hidden by the global query filter.</summary>
        bool IsDeleted { get; set; }

        /// <summary>UTC time of the soft delete.</summary>
        DateTimeOffset? DeletedOnUtc { get; set; }

        /// <summary>Id of the user that deleted the entity, if known.</summary>
        Guid? DeletedBy { get; set; }
    }
}
