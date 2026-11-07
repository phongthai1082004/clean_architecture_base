using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Behaviors
{
    public class PerformanceBehavior<TRequest, TResponse>(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly Stopwatch _timer = new();
        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            var timer = Stopwatch.StartNew();

            var response = await next();

            timer.Stop();

            var elapsedMs = timer.ElapsedMilliseconds;
            if (elapsedMs > 500)
            {
                logger.LogWarning("Long Running Request: {Name} ({Elapsed} ms) {@Request}",
                    typeof(TRequest).Name, elapsedMs, request);
            }

            return response;
        }
    }
}
