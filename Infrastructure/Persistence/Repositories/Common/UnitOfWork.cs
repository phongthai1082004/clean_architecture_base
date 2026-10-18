using System.Data;
using System.Data.Common;
using Infrastructure.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Repositories.Common
{
    public class UnitOfWork(
        DbConnection connection,
        IServiceProvider serviceProvider,
        ILogger<UnitOfWork> logger) : IUnitOfWork
    {
        private DbTransaction? _currentTransaction;

        public async Task<DbTransaction> BeginTransactionAsync(
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default)
        {
            if (_currentTransaction is not null)
                throw new InvalidOperationException("Transaction has already been opened before.");

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync(cancellationToken);

            _currentTransaction = await connection.BeginTransactionAsync(isolationLevel, cancellationToken);
            logger.LogDebug("Database transaction started with isolation level: {IsolationLevel}", isolationLevel);

            return _currentTransaction;
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction is null)
                throw new InvalidOperationException("Transaction hasn't been opened before.");

            try
            {
                await _currentTransaction.CommitAsync(cancellationToken);
                logger.LogDebug("Database transaction committed successfully.");
            }
            catch
            {
                logger.LogError("Transaction commit failed. Initiating automatic rollback...");
                await RollbackAsync(cancellationToken);
                throw;
            }
            finally
            {
                await ResetTransactionAsync();
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction is null)
                return;

            try
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
                logger.LogDebug("Database transaction rolled back successfully.");
            }
            finally
            {
                await ResetTransactionAsync();
            }
        }

        public TRepository GetRepository<TRepository>() where TRepository : notnull =>
            serviceProvider.GetRequiredService<TRepository>();

        public async ValueTask DisposeAsync()
        {
            if (_currentTransaction is not null)
            {
                await _currentTransaction.RollbackAsync();
                await ResetTransactionAsync();
            }

            await connection.DisposeAsync();
            GC.SuppressFinalize(this);
        }

        private async Task ResetTransactionAsync()
        {
            if (_currentTransaction is not null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }
}
