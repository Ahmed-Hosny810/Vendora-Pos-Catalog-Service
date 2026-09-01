
namespace Pos.CatalogService.Application.Features.ProductVariants.DTOS
{
    public class ProductVariantDto
    {
        public Guid Id { get; set; }

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

        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
