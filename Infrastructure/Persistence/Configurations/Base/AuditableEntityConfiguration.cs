using Domain.Abstractions.Auditable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Base
{
    public abstract class AuditableEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : class, IAuditable
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(e => e.CreatedOnUtc).IsRequired();
            builder.Property(e => e.CreatedBy);
            builder.Property(e => e.LastModifiedOnUtc);
            builder.Property(e => e.LastModifiedBy);
        }
    }
}
