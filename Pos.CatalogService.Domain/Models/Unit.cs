namespace Pos.CatalogService.Domain.Models
{
    public class Unit
    {
        public Guid Id { get; set; }

        public Guid? TenantId { get; set; }

        public string Name { get; set; } = null!;

        public string Symbol { get; set; } = null!;

        public bool IsDecimalAllowed { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();

        public bool IsGlobal => TenantId == null;
    }
}