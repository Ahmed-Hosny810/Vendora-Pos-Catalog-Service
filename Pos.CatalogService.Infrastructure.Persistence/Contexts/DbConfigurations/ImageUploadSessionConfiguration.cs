using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.CatalogService.Domain.Models;

namespace Pos.CatalogService.Infrastructure.Persistence.Contexts.DbConfigurations
{
    public class ImageUploadSessionConfiguration
    : IEntityTypeConfiguration<ImageUploadSession>
    {
        public void Configure(
            EntityTypeBuilder<ImageUploadSession> builder)
        {
            builder.ToTable("ImageUploadSessions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.StorageKey)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.ExpectedContentType)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.MaxSizeBytes)
                .IsRequired();

            builder.Property(x => x.ExpiresAt)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasIndex(x => x.StorageKey)
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.TenantId,
                x.ProductId
            });
        }
    }
}