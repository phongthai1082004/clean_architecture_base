using Application.Interfaces;
using Domain.Abstractions.Auditable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.Interceptors
{
    public class AuditInterceptor(ICurrentUserService currnetUserService) : SaveChangesInterceptor
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
            Guid? currentUserId = Guid.TryParse(currnetUserService.UserId, out var parsedGuid)
                ? parsedGuid
                : null;

            foreach (var entry in dbContext.ChangeTracker.Entries()) 
            {
                if (entry.State == EntityState.Deleted && entry.Entity is ISoftDelete softDeletable)
                {
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
                        auditable.CreatedOnUtc = utcNow;
                        auditable.CreatedBy = currentUserId;
                    }
                    
                    if (entry.State == EntityState.Modified)
                    {
                        auditable.LastModifiedOnUtc = utcNow;
                        auditable.LastModifiedBy = currentUserId;
                    }
                }
            }
        }
    }
}
