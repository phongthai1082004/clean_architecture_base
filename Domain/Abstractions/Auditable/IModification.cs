namespace Domain.Abstractions.Auditable
{
    public interface IModificationAuditable
    {
        DateTimeOffset? LastModifiedOnUtc { get; set; }
        Guid? LastModifiedBy { get; set; }
    }
}
