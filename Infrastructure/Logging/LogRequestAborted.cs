using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Logging
{
    public static partial class LogExtensions
    {
        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "Request aborted by client: {Path}")]
        public static partial void LogRequestAborted(this ILogger logger, PathString path);
    }
}
