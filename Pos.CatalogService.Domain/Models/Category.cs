using Pos.CatalogService.Domain.Constants;

namespace Pos.CatalogService.Domain.Models
{
    public class Category
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }

        public Guid? ParentCategoryId { get; set; }

        public string? NameAr { get; set; }

        public string NameEn { get; set; } = null!;

        public int SortOrder { get; set; }

        public bool IsVisible { get; set; } = true;

        public string Status { get; set; } = CategoryStatuses.Active;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Category? ParentCategory { get; set; }

        public ICollection<Category> Children { get; set; } = new List<Category>();

        public ICollection<Product> Products { get; set; } = new List<Product>();

        public bool IsActive => Status == CategoryStatuses.Active;

        public void Activate()
        {
            Status = CategoryStatuses.Active;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            Status = CategoryStatuses.Inactive;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Show()
        {
            IsVisible = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Hide()
        {
            IsVisible = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}