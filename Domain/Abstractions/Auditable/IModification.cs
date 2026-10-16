namespace Domain.Abstractions.Auditable
{
    public interface IModificationAuditable
    {
        DateTimeOffset? LastModifiedUtc { get; set; }
        Guid? LastModifiedBy { get; set; }
    }
}
