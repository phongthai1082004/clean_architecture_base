using Infrastructure.Persistence.Exceptions.Common;

namespace Application.Exceptions
{
    public interface IDbExceptionTranslator
    {
        DbErrorCode GetErrorCode(Exception exception);
        Exception Translate(Exception exception);
    }
}
