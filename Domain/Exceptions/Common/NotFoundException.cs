namespace Domain.Exceptions.Common
{
    public class NotFoundException(string message) : Exception(message)
    {
    }
}
