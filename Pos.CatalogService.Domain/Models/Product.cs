using Pos.CatalogService.Domain.Constants;

namespace Pos.CatalogService.Domain.Models
{
    public class Product
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }

        public Guid CategoryId { get; set; }

        public string? NameAr { get; set; }

        public string NameEn { get; set; } = null!;

        public string? Sku { get; set; }

        public string? Barcode { get; set; }

        public decimal CostPrice { get; set; }

        public decimal SellingPrice { get; set; }

        public Guid TaxRateId { get; set; }

        public Guid UnitId { get; set; }

        public bool TrackInventory { get; set; } = true;

        public string Status { get; set; } = ProductStatuses.Active;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Category Category { get; set; } = null!;

        public Unit Unit { get; set; } = null!;

        public TaxRate TaxRate { get; set; } = null!;

        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

        public bool IsActive => Status == ProductStatuses.Active;

        public bool HasVariants => Variants.Any();

        public void Activate()
        {
            Status = ProductStatuses.Active;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            Status = ProductStatuses.Inactive;
            UpdatedAt = DateTime.UtcNow;
        }

        public void EnableInventoryTracking()
        {
            TrackInventory = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void DisableInventoryTracking()
        {
            TrackInventory = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePrices(decimal costPrice, decimal sellingPrice)
        {
            CostPrice = costPrice;
            SellingPrice = sellingPrice;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}