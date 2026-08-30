using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.Products.DTOS
{
    public class IncreaseProductUsageResult
    {
        public Guid TenantId { get; set; }
        public int UsedProducts { get; set; }
        public int MaxProducts { get; set; }
    }
}
