namespace Application.Interfaces.Database
{
    /// <summary>
    /// Explicit database transaction started through <see cref="IApplicationDbContext"/>.
    /// Disposing without committing rolls the transaction back.
    /// </summary>
    public interface IDbTransactionContext : IAsyncDisposable
    {
        /// <summary>Persists all pending work in the transaction.</summary>
        Task CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>Discards all pending work in the transaction.</summary>
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
