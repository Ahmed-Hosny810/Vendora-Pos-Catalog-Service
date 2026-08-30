
namespace Pos.CatalogService.Application.Features.Products.DTOS
{
    public class DecreaseProductUsageResult
    {
        public Guid TenantId { get; set; }
        public int UsedProduct { get; set; }
    }
}
