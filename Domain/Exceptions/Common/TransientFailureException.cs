namespace Domain.Exceptions.Common
{
    public class TransientFailureException : Exception
    {
        public TransientFailureException(string message)
           : base(message)
        {
        }
        public TransientFailureException(string message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
