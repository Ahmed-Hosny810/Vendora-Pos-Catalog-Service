using Pos.CatalogService.Application.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.Products.Queries.GetAllQuery
{
    public class GetProductsQueryParameter : RequestParameter<ProductOrderKey>
    {
        public ProductFilter? Filter { get; set; }

        public ProductIncludes? Includes { get; set; }
    }

    public class ProductFilter
    {
        public Guid? Id { get; set; }

        public Guid? CategoryId { get; set; }

        public Guid? UnitId { get; set; }

        public Guid? TaxRateId { get; set; }

        public string? Search { get; set; }

        public string? NameAr { get; set; }

        public string? NameEn { get; set; }

        public string? Sku { get; set; }

        public string? Barcode { get; set; }

        public bool? TrackInventory { get; set; }

        public string? Status { get; set; }
    }

    public class ProductIncludes
    {
        public bool Category { get; set; }

        public bool Unit { get; set; }

        public bool TaxRate { get; set; }

        public bool Images { get; set; }

        public bool Variants { get; set; }
    }

    public enum ProductOrderKey
    {
        CreatedAt,
        UpdatedAt,
        NameAr,
        NameEn,
        Sku,
        Barcode,
        CostPrice,
        SellingPrice,
        Status
    }
}
