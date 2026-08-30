
using Pos.CatalogService.Application.Features.Products.DTOS;
using Pos.CatalogService.Application.Wrappers;

namespace Pos.CatalogService.Application.Interfaces.Clients
{
    public interface ITenantBillingClient
    {
        Task<Result<IncreaseProductUsageResult>> IncreaseProductUsageAsync(ProductUsageRequest request, CancellationToken cancellationToken);
        Task<Result<DecreaseProductUsageResult>> DecreaseProductUsageAsync(ProductUsageRequest request, CancellationToken cancellationToken);
    }
}
