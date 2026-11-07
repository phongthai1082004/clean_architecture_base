using Domain.Enums.Idempotency;

namespace Domain.Exceptions.Idempotency
{
    public class MissingIdempotencyKeyException : Exception
    {
        public MissingIdempotencyKeyException() : base("Missing required 'Idempotency-Key' header.")
        {
        }
    }
}
