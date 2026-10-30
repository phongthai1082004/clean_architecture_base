using Domain.Entities;
using Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations
{
    public class OutboxMessageConfiguration : AuditableEntityConfiguration<OutboxMessage>
    {
        public override void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            base.Configure(builder);

            builder.ToTable("outbox_messages");

            builder.Property(m => m.EventType).HasMaxLength(255).IsRequired();
            builder.Property(m => m.Payload).IsRequired();
            builder.Property(m => m.OccurredOn).IsRequired();
        }
    }
}
