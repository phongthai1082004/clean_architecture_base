using Domain.Entities.Users;
using Domain.Enums.Users;
using Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : SoftDeleteEntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);

            builder.ToTable("users");
            builder.Ignore(u => u.DomainEvents);

            builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.FullName).HasMaxLength(150).IsRequired();
            builder.Property(u => u.PasswordHash).HasMaxLength(255);
            builder.Property(u => u.PhoneNumber).HasMaxLength(50);
            builder.Property(u => u.AvatarUrl).HasMaxLength(500).IsRequired().HasDefaultValue("");
            builder.Property(u => u.Role).IsRequired().HasDefaultValue(UserRole.User);

            builder.HasMany(u => u.Tokens).WithOne().HasForeignKey(t => t.UserId);
            builder.HasMany(u => u.ExternalTokens).WithOne().HasForeignKey(t => t.UserId);
        }
    }
}
