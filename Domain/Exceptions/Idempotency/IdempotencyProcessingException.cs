using Domain.Enums.Idempotency;

namespace Domain.Exceptions.Idempotency
{
    public class IdempotencyProcessingException : Exception
    {
        public IdempotencyProcessingException(IdempotencyStatus status) : base($"This request is {status}")
        {
            
        }
    }
}
