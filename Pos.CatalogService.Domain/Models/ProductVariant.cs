using Pos.CatalogService.Domain.Constants;

namespace Pos.CatalogService.Domain.Models
{
    public class ProductVariant
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }

        public Guid ProductId { get; set; }

        public string? Sku { get; set; }

        public string? Barcode { get; set; }

        public string Name { get; set; } = null!;

        public decimal SellingPrice { get; set; }

        public decimal CostPrice { get; set; }

        public string? OptionKey1 { get; set; }

        public string? OptionValue1 { get; set; }

        public string? OptionKey2 { get; set; }

        public string? OptionValue2 { get; set; }

        public string Status { get; set; } = ProductVariantStatuses.Active;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Product Product { get; set; } = null!;

        public bool IsActive => Status == ProductVariantStatuses.Active;

        public void Activate()
        {
            Status = ProductVariantStatuses.Active;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            Status = ProductVariantStatuses.Inactive;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePrices(decimal costPrice, decimal sellingPrice)
        {
            if (costPrice < 0)
                throw new InvalidOperationException("Cost price cannot be negative.");

            if (sellingPrice < 0)
                throw new InvalidOperationException("Selling price cannot be negative.");

            CostPrice = costPrice;
            SellingPrice = sellingPrice;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}