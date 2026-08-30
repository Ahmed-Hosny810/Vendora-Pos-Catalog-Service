using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.Products.DTOS
{
    public class ProductDto
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }

        public Guid CategoryId { get; set; }

        public string? CategoryNameEn { get; set; }

        public string? CategoryNameAr { get; set; }

        public string? NameAr { get; set; }

        public string NameEn { get; set; } = null!;

        public string? Sku { get; set; }

        public string? Barcode { get; set; }

        public decimal CostPrice { get; set; }

        public decimal SellingPrice { get; set; }

        public Guid TaxRateId { get; set; }

        public string? TaxRateName { get; set; }

        public decimal? TaxRateValue { get; set; }

        public Guid UnitId { get; set; }

        public string? UnitName { get; set; }

        public string? UnitSymbol { get; set; }

        public bool TrackInventory { get; set; }

        public string Status { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public IEnumerable<ProductVariantDto> Variants { get; set; } = new List<ProductVariantDto>();

        public IEnumerable<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();
    }
}
