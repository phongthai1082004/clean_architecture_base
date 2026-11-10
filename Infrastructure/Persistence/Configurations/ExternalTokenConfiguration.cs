using Domain.Entities.Users;
using Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ExternalTokenConfiguration : AuditableEntityConfiguration<ExternalToken>
    {
        public override void Configure(EntityTypeBuilder<ExternalToken> builder)
        {
            base.Configure(builder);

            builder.ToTable("external_tokens");

            builder.Property(t => t.Provider).IsRequired().HasConversion<int>();
            builder.Property(t => t.ProviderUserId).HasMaxLength(100).IsRequired();
            builder.Property(t => t.AccessToken).IsRequired();
            builder.Property(t => t.ExpiresAt);
        }
    }
}
