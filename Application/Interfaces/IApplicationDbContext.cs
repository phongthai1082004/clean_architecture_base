namespace Application.Interfaces
{
    public interface IApplicationDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<IDbTransactionContext> BeginTransactionAsync(CancellationToken cancellationToken = default);
    }
}
