using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.CatalogService.Domain.Constants;
using Pos.CatalogService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Infrastructure.Persistence.Contexts.DbConfigurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products", "catalog");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TenantId)
                .IsRequired();

            builder.Property(x => x.CategoryId)
                .IsRequired();

            builder.Property(x => x.NameAr)
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(x => x.NameEn)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Sku)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(x => x.Barcode)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(x => x.CostPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.SellingPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.TaxRateId)
                .IsRequired();

            builder.Property(x => x.UnitId)
                .IsRequired();

            builder.Property(x => x.TrackInventory)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasMaxLength(30)
                .HasDefaultValue(ProductStatuses.Active)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired(false);

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Unit)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.UnitId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.TaxRate)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.TaxRateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Variants)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Images)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.TenantId);

            builder.HasIndex(x => x.CategoryId);

            builder.HasIndex(x => x.UnitId);

            builder.HasIndex(x => x.TaxRateId);

            builder.HasIndex(x => new { x.TenantId, x.NameEn });

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
