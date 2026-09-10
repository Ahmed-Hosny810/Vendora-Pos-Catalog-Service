using Pos.CatalogService.Domain.Constants;
using Pos.CatalogService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.IntegrationTests
{
    public static class TestDataFactory
    {
        public static readonly Guid TenantId =
            Guid.Parse("11111111-1111-1111-1111-111111111111");

        public static readonly Guid OtherTenantId =
            Guid.Parse("22222222-2222-2222-2222-222222222222");

        public static readonly Guid UserId =
            Guid.Parse("33333333-3333-3333-3333-333333333333");

        public static Category CreateCategory(
            Guid? tenantId = null,
            string name = "Food",
            string status = CategoryStatuses.Active)
        {
            return new Category
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId ?? TenantId,
                NameAr = "طعام",
                NameEn = name,
                SortOrder = 1,
                IsVisible = true,
                Status = status,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static Domain.Models.Unit CreateUnit(
            Guid? tenantId = null,
            string name = "Piece",
            string symbol = "PC")
        {
            return new Domain.Models.Unit
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId ?? TenantId,
                Name = name,
                Symbol = symbol,
                IsDecimalAllowed = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static TaxRate CreateTaxRate(
            Guid? tenantId = null,
            string name = "VAT 14%",
            decimal rate = 14,
            bool isDefault = false,
            bool isActive = true)
        {
            return new TaxRate
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId ?? TenantId,
                Name = name,
                Rate = rate,
                IsDefault = isDefault,
                IsActive = isActive,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static Product CreateProduct(
            Guid categoryId,
            Guid unitId,
            Guid taxRateId,
            Guid? tenantId = null,
            string? sku = "PRODUCT-001",
            string? barcode = "100000000001")
        {
            return new Product
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId ?? TenantId,
                CategoryId = categoryId,
                UnitId = unitId,
                TaxRateId = taxRateId,
                NameAr = "منتج اختبار",
                NameEn = "Test Product",
                Sku = sku,
                Barcode = barcode,
                CostPrice = 50,
                SellingPrice = 75,
                TrackInventory = true,
                Status = ProductStatuses.Active,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static ProductVariant CreateProductVariant(
            Guid productId,
            Guid? tenantId = null,
            string? sku = "VARIANT-001",
            string? barcode = "200000000001")
        {
            return new ProductVariant
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId ?? TenantId,
                ProductId = productId,
                Name = "Large",
                Sku = sku,
                Barcode = barcode,
                SellingPrice = 90,
                CostPrice = 60,
                OptionKey1 = "Size",
                OptionValue1 = "Large",
                Status = ProductVariantStatuses.Active,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static ImageUploadSession CreateUploadSession(
            Guid productId,
            Guid? tenantId = null,
            string storageKey = "test/product.jpg")
        {
            return new ImageUploadSession
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId ?? TenantId,
                ProductId = productId,
                StorageKey = storageKey,
                ExpectedContentType = "image/jpeg",
                MaxSizeBytes = 5 * 1024 * 1024,
                Status = ImageUploadStatuses.Pending,
                ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10),
                CreatedAt = DateTimeOffset.UtcNow
            };
        }
    }
}
