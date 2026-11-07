namespace Domain.Exceptions.Idempotency
{
    public class UnprocessableEntityException(string message) : Exception(message)
    {
    }
}
