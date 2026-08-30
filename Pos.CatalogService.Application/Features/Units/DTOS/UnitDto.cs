using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.Units.DTOS
{
    public class UnitDto
    {
        public Guid Id { get; set; }

        public Guid? TenantId { get; set; }

        public string Name { get; set; } = null!;

        public string Symbol { get; set; } = null!;

        public bool IsDecimalAllowed { get; set; }

        public bool IsGlobal { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
