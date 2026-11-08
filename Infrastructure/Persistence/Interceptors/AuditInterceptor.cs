using Application.Interfaces.Services;
using Domain.Abstractions.Auditable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Interceptors
{
    public class AuditInterceptor(
        ICurrentUserService currnetUserService,
        ILogger<AuditInterceptor> logger) : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
        {
            Update(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            Update(eventData.Context);
            return base.SavingChanges(eventData, result);
        }
        private void Update(DbContext? dbContext)
        {
            if (dbContext == null) return;
            var utcNow = DateTime.UtcNow;
            var currentUserId = currnetUserService.UserId;

            foreach (var entry in dbContext.ChangeTracker.Entries()) 
            {
                if (entry.State == EntityState.Deleted && entry.Entity is ISoftDelete softDeletable)
                {
                    if(logger.IsEnabled(LogLevel.Debug))
                        logger.LogDebug("Soft-delete {Entity}", entry.Metadata.ClrType.Name);
                    entry.State = EntityState.Modified;
                    softDeletable.IsDeleted = true;
                    softDeletable.DeletedOnUtc = utcNow;
                    softDeletable.DeletedBy = currentUserId;
                    continue;
                }

                if (entry.Entity is IAuditable auditable)
                {
                    if (entry.State == EntityState.Added)
                    {
                        logger.LogDebug("Stamp created {Entity}", entry.Metadata.ClrType.Name);
                        auditable.CreatedOnUtc = utcNow;
                        auditable.CreatedBy = currentUserId;
                    }
                    
                    if (entry.State == EntityState.Modified)
                    {
                        logger.LogDebug("Stamp modified {Entity}", entry.Metadata.ClrType.Name);
                        auditable.LastModifiedOnUtc = utcNow;
                        auditable.LastModifiedBy = currentUserId;
                    }
                }
            }
        }
    }
}
