using System.Data;
using System.Data.Common;

namespace Infrastructure.Abstractions
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        Task<DbTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);
        Task CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
        TRepository GetRepository<TRepository>() where TRepository : notnull;
    }
}
