using Domain.Entities.Users;
using Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class AuditLogConfiguration : AuditableEntityConfiguration<AuditLog>
    {
        public override void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            base.Configure(builder);

            builder.ToTable("audit_logs");

            builder.Property(a => a.Action).IsRequired();
            builder.Property(a => a.TableName).HasMaxLength(100).IsRequired();
            builder.Property(a => a.RecordId).HasMaxLength(100).IsRequired();
            builder.Property(a => a.Message).IsRequired();

            builder.HasOne<User>().WithMany().HasForeignKey(a => a.UserId);
        }
    }
}
