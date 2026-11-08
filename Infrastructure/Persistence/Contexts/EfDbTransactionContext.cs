using Application.Interfaces.Database;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence.Contexts
{
    public sealed class EfDbTransactionContext(IDbContextTransaction transaction) : IDbTransactionContext
    {
        public Task CommitAsync(CancellationToken cancellationToken = default) => transaction.CommitAsync(cancellationToken);
        public Task RollbackAsync(CancellationToken cancellationToken = default) => transaction.RollbackAsync(cancellationToken);

        public async ValueTask DisposeAsync()
        {
            await transaction.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}
