namespace Domain.Abstractions.Auditable
{
    /// <summary>Entity records who and when it was created.</summary>
    public interface ICreationAuditable
    {
        /// <summary>UTC creation time, set by the audit interceptor on insert.</summary>
        DateTimeOffset CreatedOnUtc { get; set; }

        /// <summary>Id of the user that created the entity, if known.</summary>
        Guid? CreatedBy { get; set; }
    }
}
