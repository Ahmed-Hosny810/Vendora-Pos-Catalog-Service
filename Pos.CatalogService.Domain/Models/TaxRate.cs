namespace Pos.CatalogService.Domain.Models
{
    public class TaxRate
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }

        public string Name { get; set; } = null!;

        public decimal Rate { get; set; }

        public bool IsDefault { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsDefault()
        {
            if (!IsActive)
                throw new InvalidOperationException("Inactive tax rate cannot be set as default.");

            IsDefault = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UnmarkAsDefault()
        {
            IsDefault = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}