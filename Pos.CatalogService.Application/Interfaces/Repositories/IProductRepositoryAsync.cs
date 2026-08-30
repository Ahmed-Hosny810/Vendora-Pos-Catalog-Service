using Pos.CatalogService.Application.Features.Products.Queries.GetAllQuery;
using Pos.CatalogService.Application.Wrappers;
using Pos.CatalogService.Domain.Models;

namespace Pos.CatalogService.Application.Interfaces.Repositories
{
    public interface IProductRepositoryAsync : IGenericRepositoryAsync<Product, Guid>
    {
        Task<Product?> GetProductByIdAndTenantIdForUpdateAsync(
            Guid tenantId,
            Guid productId,
            CancellationToken cancellationToken);

        Task<Product?> GetProductByIdAndTenantIdAsync(
            Guid tenantId,
            Guid productId,
            ProductIncludes? includes,
            CancellationToken cancellationToken);

        Task<PagedResponse<IEnumerable<Product>>> GetProductsPagedResponseAsync(
            Guid tenantId,
            ProductFilter? filter,
            ProductIncludes? includes,
            ProductOrderKey orderKey,
            bool orderDescending,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken);

        Task<bool> IsSkuExistsAsync(
            Guid tenantId,
            string sku,
            CancellationToken cancellationToken);

        Task<bool> IsBarcodeExistsAsync(
            Guid tenantId,
            string barcode,
            CancellationToken cancellationToken);

        Task<bool> IsSkuExistsForAnotherProductAsync(
            Guid tenantId,
            Guid productId,
            string sku,
            CancellationToken cancellationToken);

        Task<bool> IsBarcodeExistsForAnotherProductAsync(
            Guid tenantId,
            Guid productId,
            string barcode,
            CancellationToken cancellationToken);
    }
}
