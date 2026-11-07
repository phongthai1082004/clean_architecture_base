using Domain.Entities.Notifications;
using Domain.Entities.Users;
using Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations
{
    public class NotificationConfiguration : AuditableEntityConfiguration<Notification>
    {
        public override void Configure(EntityTypeBuilder<Notification> builder)
        {
            base.Configure(builder);

            builder.ToTable("notifications");

            builder.Property(n => n.Title).HasMaxLength(200).IsRequired();
            builder.Property(n => n.Message).IsRequired();
            builder.Property(n => n.Type).IsRequired();

            builder.HasOne<User>().WithMany().HasForeignKey(n => n.UserId);
        }
    }
}
