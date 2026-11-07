using Domain.Entities.Idempotency;
using Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class IdempotencyRecordConfiguration : AuditableEntityConfiguration<IdempotencyRecord>
    {
        public override void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
        {
            base.Configure(builder);

            builder.ToTable("idempotency_records");

            builder.Property(r => r.Key).HasMaxLength(255).IsRequired();
            builder.Property(r => r.Endpoint).HasMaxLength(500).IsRequired();
            builder.Property(r => r.RequestHash).HasMaxLength(64).IsRequired();
            builder.Property(r => r.ExpiresAt).IsRequired();

            builder.HasIndex(r => new { r.UserId, r.Endpoint, r.Key }).IsUnique();
            builder.HasIndex(r => r.ExpiresAt);
        }
    }
}
