using Application.Exceptions;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Windows.Input;

namespace Application.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse>(
        ILogger<TransactionBehavior<TRequest, TResponse>> logger,
        IApplicationDbContext context,
        IDbExceptionTranslator translator) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is not ICommand)
                return await next();

            var requestName = typeof(TRequest).Name;
            await using var transaction = await context.BeginTransactionAsync(cancellationToken);
            try
            {
                var response = await next();
                await context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("Committed database transaction for {RequestName}", requestName);
                }

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Transaction failed and rolling back for {RequestName}", requestName);

                try
                {
                    await transaction.RollbackAsync(cancellationToken);
                }
                catch (Exception rollbackEx)
                {
                    logger.LogError(rollbackEx, "Failed to rollback transaction for {RequestName}", requestName);
                }

                throw translator.Translate(ex);
            }
        }
    }
}
