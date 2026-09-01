using Pos.CatalogService.Domain.Models;

namespace Pos.CatalogService.Application.Interfaces.Repositories
{
    public interface IProductVariantRepositoryAsync: IGenericRepositoryAsync<ProductVariant, Guid>
    {
        Task<ProductVariant?> GetProductVariantByIdAndTenantIdAsync(
            Guid tenantId,
            Guid variantId,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<ProductVariant>> GetProductVariantsByProductIdAsync(
            Guid tenantId,
            Guid productId,
            CancellationToken cancellationToken);

        Task<bool> IsSkuExistsAsync(
            Guid tenantId,
            string sku,
            CancellationToken cancellationToken);

        Task<bool> IsBarcodeExistsAsync(
            Guid tenantId,
            string barcode,
            CancellationToken cancellationToken);

        Task<bool> IsSkuExistsForAnotherVariantAsync(
            Guid tenantId,
            Guid variantId,
            string sku,
            CancellationToken cancellationToken);

        Task<bool> IsBarcodeExistsForAnotherVariantAsync(
            Guid tenantId,
            Guid variantId,
            string barcode,
            CancellationToken cancellationToken);
    }
}
