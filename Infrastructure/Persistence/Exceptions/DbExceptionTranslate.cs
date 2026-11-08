using Application.Exceptions;
using Domain.Exceptions.Common;
using Infrastructure.Persistence.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Persistence.Exceptions
{
    public class DbExceptionTranslate : IDbExceptionTranslator
    {
        public DbErrorCode GetErrorCode(Exception exception)
        {
            var pgEx = ExtractPostgresException(exception);
            if (pgEx is null)
            {
                return DbErrorCode.None;
            }

            return pgEx.SqlState switch
            {
                PostgresErrorCodes.UniqueViolation => DbErrorCode.UniqueViolation,          
                PostgresErrorCodes.ForeignKeyViolation => DbErrorCode.ForeignKeyViolation,  
                PostgresErrorCodes.NotNullViolation => DbErrorCode.NotNullViolation,       
                PostgresErrorCodes.CheckViolation => DbErrorCode.CheckViolation,           
                PostgresErrorCodes.DeadlockDetected => DbErrorCode.Deadlock,                
                PostgresErrorCodes.QueryCanceled => DbErrorCode.Timeout,                   
                _ => DbErrorCode.Unknown
            };
        }

        public Exception Translate(Exception exception)
        {
            var code = GetErrorCode(exception);

            return code switch
            {
                DbErrorCode.UniqueViolation => new ConflictException("The specified record already exists.", exception),
                DbErrorCode.ForeignKeyViolation => new BadRequestException("The referenced record does not exist or is currently in use.", exception),
                DbErrorCode.NotNullViolation => new BadRequestException("A required field is missing.", exception),
                DbErrorCode.CheckViolation => new BadRequestException("The provided data violates a business or check constraint.", exception),
                DbErrorCode.Deadlock => new TransientFailureException("A concurrency conflict occurred while processing the request. Please try again.", exception),
                _ => exception
            };
        }
        private static PostgresException? ExtractPostgresException(Exception exception)
        {
            if (exception is PostgresException pgEx)
            {
                return pgEx;
            }

            if (exception is DbUpdateException dbEx && dbEx.InnerException is PostgresException innerPgEx)
            {
                return innerPgEx;
            }

            return exception.InnerException as PostgresException;
        }
    }
}
