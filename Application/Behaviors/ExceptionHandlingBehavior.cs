using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Behaviors
{
    public class ExceptionHandlingBehavior<TRequest, TResponse>(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger) 
        : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning("Request {RequestName} was cancelled by client.", typeof(TRequest).Name);
                throw;
            }
        }
    }
}
