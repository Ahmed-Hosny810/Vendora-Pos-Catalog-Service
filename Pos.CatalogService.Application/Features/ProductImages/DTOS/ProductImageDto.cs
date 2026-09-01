
namespace Pos.CatalogService.Application.Features.ProductImages.DTOS
{
    public class ProductImageDto
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }

        public Guid ProductId { get; set; }

        public string ImageUrl { get; set; } = null!;

        public int SortOrder { get; set; }

        public bool IsMain { get; set; }

        public DateTime CreatedAt { get; set; }
    }

}
