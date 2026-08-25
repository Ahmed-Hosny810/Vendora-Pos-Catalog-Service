using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.CatalogService.Domain.Constants;
using Pos.CatalogService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Infrastructure.Persistence.Contexts.DbConfigurations
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.ToTable("ProductVariants", "catalog");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TenantId)
                .IsRequired();

            builder.Property(x => x.ProductId)
                .IsRequired();

            builder.Property(x => x.Sku)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(x => x.Barcode)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.SellingPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.CostPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.OptionKey1)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(x => x.OptionValue1)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(x => x.OptionKey2)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(x => x.OptionValue2)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(x => x.Status)
                .HasMaxLength(30)
                .HasDefaultValue(ProductVariantStatuses.Active)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired(false);

            builder.HasOne(x => x.Product)
                .WithMany(x => x.Variants)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.TenantId);

            builder.HasIndex(x => x.ProductId);

            builder.HasIndex(x => new { x.TenantId, x.ProductId });

            builder.HasIndex(x => new { x.TenantId, x.Status });

            builder.HasIndex(x => new { x.TenantId, x.Sku })
                .IsUnique()
                .HasFilter("[Sku] IS NOT NULL");

            builder.HasIndex(x => new { x.TenantId, x.Barcode })
                .IsUnique()
                .HasFilter("[Barcode] IS NOT NULL");
        }
    }
}
