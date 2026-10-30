using Domain.Entities;
using Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations
{
    public class FileMetadataConfiguration : AuditableEntityConfiguration<FileMetadata>
    {
        public override void Configure(EntityTypeBuilder<FileMetadata> builder)
        {
            base.Configure(builder);

            builder.ToTable("file_metadata");

            builder.Property(f => f.FileName).HasMaxLength(255).IsRequired();
            builder.Property(f => f.FilePath).HasMaxLength(500).IsRequired();
            builder.Property(f => f.ContentType).HasMaxLength(100).IsRequired();
        }
    }
}
