using Application.Interfaces.Database;
using Domain.Entities;
using Domain.Entities.Idempotency;
using Domain.Entities.Notifications;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Contexts
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<UserToken> UserTokens => Set<UserToken>();
        public DbSet<ExternalToken> ExternalTokens => Set<ExternalToken>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
        public async Task<IDbTransactionContext> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            var efTx = await Database.BeginTransactionAsync(cancellationToken);
            return new EfDbTransactionContext(efTx);
        }
    }
}
