using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) 
        : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            if (logger.IsEnabled(LogLevel.Information)) logger.LogInformation("Processing request {RequestName}", requestName);
            var response = await next();
            if (logger.IsEnabled(LogLevel.Information)) logger.LogInformation("Completed request {RequestName}", requestName);
            return response;
        }
    }
}
