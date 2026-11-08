namespace Application.Interfaces.Database
{
    /// <summary>Minimal persistence abstraction so Application does not depend on EF Core.</summary>
    public interface IApplicationDbContext
    {
        /// <summary>Flushes all tracked changes to the database.</summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>Starts an explicit transaction; commit or roll back the returned context.</summary>
        Task<IDbTransactionContext> BeginTransactionAsync(CancellationToken cancellationToken = default);
    }
}
