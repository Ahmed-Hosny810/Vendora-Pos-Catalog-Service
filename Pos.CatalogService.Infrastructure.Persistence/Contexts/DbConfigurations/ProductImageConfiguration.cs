using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.CatalogService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Infrastructure.Persistence.Contexts.DbConfigurations
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.ToTable("ProductImages", "catalog");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TenantId)
                .IsRequired();

            builder.Property(x => x.ProductId)
                .IsRequired();

            builder.Property(x => x.ImageUrl)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.SortOrder)
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(x => x.IsMain)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasOne(x => x.Product)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.TenantId);

            builder.HasIndex(x => x.ProductId);

            builder.HasIndex(x => new { x.TenantId, x.ProductId });

            builder.HasIndex(x => new { x.TenantId, x.ProductId, x.IsMain });
        }
    }
}
