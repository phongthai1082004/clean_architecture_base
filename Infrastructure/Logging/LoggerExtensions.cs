using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Logging
{
    public static class LoggerExtensions
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
            logger.Log(
                logLevel,
                exception,
                "[{FileName}:{LineNumber} -> {Method}] {Message}",
                message,
                Path.GetFileName(sourceFilePath),
                sourceLineNumber,
                memberName);
        }
    }
}
