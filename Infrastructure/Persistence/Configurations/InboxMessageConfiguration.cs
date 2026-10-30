using Domain.Entities;
using Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations
{
    public class InboxMessageConfiguration : AuditableEntityConfiguration<InboxMessage>
    {
        public override void Configure(EntityTypeBuilder<InboxMessage> builder)
        {
            base.Configure(builder);

            builder.ToTable("inbox_messages");

            builder.Property(m => m.ProcessedAt).IsRequired();
        }
    }
}
