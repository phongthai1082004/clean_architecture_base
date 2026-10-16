namespace Domain.Abstractions.Auditable
{
    public interface ICreationAuditable
    {
        DateTimeOffset CreatedOnUtc { get; set; }
        Guid? CreatedBy { get; set; }
    }
}
