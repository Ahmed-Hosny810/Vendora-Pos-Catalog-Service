
namespace Pos.CatalogService.Application.Features.Categories.DTOS
{
    public class CategoryDto
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }

        public Guid? ParentCategoryId { get; set; }

        public string? NameAr { get; set; }

        public string NameEn { get; set; } = null!;

        public int SortOrder { get; set; }

        public bool IsVisible { get; set; }

        public string Status { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
