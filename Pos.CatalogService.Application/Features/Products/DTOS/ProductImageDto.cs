
namespace Pos.CatalogService.Application.Features.Products.DTOS
{
    public class ProductImageDto
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public string ImageUrl { get; set; } = null!;

        public int SortOrder { get; set; }

        public bool IsMain { get; set; }
    }
}
