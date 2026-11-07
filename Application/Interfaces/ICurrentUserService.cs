namespace Application.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? Idempotency {  get; }
        string Endpoint { get; }
        string? Payload { get; }
    }
}