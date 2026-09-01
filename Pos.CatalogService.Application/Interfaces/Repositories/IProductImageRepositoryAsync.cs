using Pos.CatalogService.Domain.Models;

namespace Pos.CatalogService.Application.Interfaces.Repositories
{
    public interface IProductImageRepositoryAsync
       : IGenericRepositoryAsync<ProductImage, Guid>
    {
        Task<ProductImage?> GetProductImageByIdAndTenantIdAsync(
            Guid tenantId,
            Guid imageId,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<ProductImage>> GetProductImagesByProductIdAsync(
            Guid tenantId,
            Guid productId,
            CancellationToken cancellationToken);

        Task<ProductImage?> GetMainProductImageAsync(
            Guid tenantId,
            Guid productId,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<ProductImage>> GetProductImagesForSetMainAsync(
            Guid tenantId,
            Guid productId,
            CancellationToken cancellationToken);
    }
}