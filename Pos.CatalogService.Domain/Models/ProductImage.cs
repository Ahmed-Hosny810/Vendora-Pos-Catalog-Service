using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Domain.Models
{
    public class ProductImage
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }

        public Guid ProductId { get; set; }

        public string ImageUrl { get; set; } = null!;

        public string StorageKey { get; set; } = null!;

        public int SortOrder { get; set; }

        public bool IsMain { get; set; }

        public DateTime CreatedAt { get; set; }

        public Product Product { get; set; } = null!;

        public void MarkAsMain()
        {
            IsMain = true;
        }

        public void UnmarkAsMain()
        {
            IsMain = false;
        }
    }
}
