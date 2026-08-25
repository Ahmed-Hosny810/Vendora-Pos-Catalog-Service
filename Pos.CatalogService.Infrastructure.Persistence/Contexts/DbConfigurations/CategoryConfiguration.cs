using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.CatalogService.Domain.Constants;
using Pos.CatalogService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Infrastructure.Persistence.Contexts.DbConfigurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories", "catalog");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TenantId)
                .IsRequired();

            builder.Property(x => x.ParentCategoryId)
                .IsRequired(false);

            builder.Property(x => x.NameAr)
                .HasMaxLength(150)
                .IsRequired(false);

            builder.Property(x => x.NameEn)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.SortOrder)
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(x => x.IsVisible)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasMaxLength(30)
                .HasDefaultValue(CategoryStatuses.Active)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired(false);

            builder.HasOne(x => x.ParentCategory)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Products)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.TenantId);

            builder.HasIndex(x => new { x.TenantId, x.ParentCategoryId });

            builder.HasIndex(x => new { x.TenantId, x.NameEn });

            builder.HasIndex(x => new { x.TenantId, x.Status });
        }
    }
}
