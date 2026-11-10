using Domain.Entities.Users;
using Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations
{
    public class UserTokenConfiguration : AuditableEntityConfiguration<UserToken>
    {
        public override void Configure(EntityTypeBuilder<UserToken> builder)
        {
            base.Configure(builder);

            builder.ToTable("user_tokens");

            builder.Property(t => t.TokenValue).IsRequired();
            builder.Property(t => t.Type).IsRequired().HasConversion<int>();
            builder.Property(t => t.ExpiresAt).IsRequired();
        }
    }
}
