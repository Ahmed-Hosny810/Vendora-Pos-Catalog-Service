using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.TaxRates.DTOS
{
    public class TaxRateDto
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }

        public string Name { get; set; } = null!;

        public decimal Rate { get; set; }

        public bool IsDefault { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
