namespace Domain.Abstractions.Auditable
{
    /// <summary>Entity records who and when it was last modified.</summary>
    public interface IModificationAuditable
    {
        /// <summary>UTC time of the last update, set by the audit interceptor.</summary>
        DateTimeOffset? LastModifiedOnUtc { get; set; }

        /// <summary>Id of the user that last updated the entity, if known.</summary>
        Guid? LastModifiedBy { get; set; }
    }
}
