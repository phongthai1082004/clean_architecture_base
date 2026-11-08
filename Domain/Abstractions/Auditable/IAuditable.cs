namespace Domain.Abstractions.Auditable
{
    /// <summary>Entity carries both creation and modification audit columns.</summary>
    public interface IAuditable : ICreationAuditable, IModificationAuditable
    {
    }
}
