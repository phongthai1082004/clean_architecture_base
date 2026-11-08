namespace Application.Interfaces
{
    /// <summary>
    /// Access to the current HTTP request identity and payload.
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>Authenticated user id, or <c>null</c> when the request is anonymous.</summary>
        Guid? UserId { get; }

        /// <summary>Raw value of the <c>Idempotency-Key</c> request header.</summary>
        string? Idempotency {  get; }

        /// <summary>Request identity ("METHOD /path?query") used to scope idempotency keys.</summary>
        string Endpoint { get; }

        /// <summary>Raw request body, cached after the first read; used to hash idempotent requests.</summary>
        string? Payload { get; }
    }
}