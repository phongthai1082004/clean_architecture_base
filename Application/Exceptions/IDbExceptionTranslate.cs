using Infrastructure.Persistence.Exceptions.Common;

namespace Application.Exceptions
{
    /// <summary>Translates low-level database exceptions into domain-level exceptions.</summary>
    public interface IDbExceptionTranslator
    {
        /// <summary>Classifies the underlying Postgres error, or <see cref="DbErrorCode.None"/> if it is not a database error.</summary>
        DbErrorCode GetErrorCode(Exception exception);

        /// <summary>Converts a database exception into a user-facing domain exception.</summary>
        Exception Translate(Exception exception);
    }
}
