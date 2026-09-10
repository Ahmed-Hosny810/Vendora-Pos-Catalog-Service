
using Pos.CatalogService.Application.Features.Products.DTOS;
using Pos.CatalogService.Application.Interfaces.Clients;
using Pos.CatalogService.Application.Wrappers;

namespace Pos.CatalogService.IntegrationTests.Fakes
{
    public class FakeTenantBillingClient : ITenantBillingClient
    {
        public bool ShouldFailIncrease { get; set; }

        public bool ShouldFailDecrease { get; set; }

        public int IncreaseCallCount { get; private set; }

        public int DecreaseCallCount { get; private set; }

        public Task<Result<IncreaseProductUsageResult>>IncreaseProductUsageAsync(ProductUsageRequest request,
                CancellationToken cancellationToken)
        {
            IncreaseCallCount++;

            if (ShouldFailIncrease)
            {
                return Task.FromResult(
                    Result<IncreaseProductUsageResult>.Failure(
                        "Maximum product limit reached."));
            }

            return Task.FromResult(
                Result<IncreaseProductUsageResult>.Success(
                    new IncreaseProductUsageResult
                    {
                        TenantId = request.TenantId,
                        UsedProducts = 1,
                        MaxProducts = 100
                    }));
        }

        public Task<Result<DecreaseProductUsageResult>>DecreaseProductUsageAsync(ProductUsageRequest request,
                CancellationToken cancellationToken)
        {
            DecreaseCallCount++;

            if (ShouldFailDecrease)
            {
                return Task.FromResult(
                    Result<DecreaseProductUsageResult>.Failure(
                        "Failed to decrease product usage."));
            }

            return Task.FromResult(
                Result<DecreaseProductUsageResult>.Success(
                    new DecreaseProductUsageResult
                    {
                        TenantId = request.TenantId,
                        UsedProduct = 0
                    }));
        }
    }
}
