using Domain.Abstractions.Auditable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Base
{
    public abstract class SoftDeleteEntityConfiguration<TEntity> : AuditableEntityConfiguration<TEntity>
        where TEntity : class, IAuditable, ISoftDelete
    {
        public override void Configure(EntityTypeBuilder<TEntity> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.IsDeleted).HasDefaultValue(false);
            builder.Property(e => e.DeleteOnUtc);
            builder.Property(e => e.DeletedBy);
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
