using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Logging
{
    public static partial class LoggingExtensions
    {
        public static void LogWithLocation(
            this ILogger logger,
            LogLevel logLevel,
            string message,
            Exception? exception = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!logger.IsEnabled(logLevel))
            {
                return;
            }

            logger.Log(
                logLevel,
                exception,
                "[{FileName}:{LineNumber} -> {Method}] {Message}",
                System.IO.Path.GetFileName(sourceFilePath),
                sourceLineNumber,
                memberName,
                message);
        }
    }
}
