using Microsoft.EntityFrameworkCore;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Domain.Models;
using Pos.CatalogService.Infrastructure.Persistence.Contexts;

namespace Pos.CatalogService.Infrastructure.Persistence.Repositories
{
    public class ProductImageRepositoryAsync
        : GenericRepositoryAsync<ProductImage, Guid>, IProductImageRepositoryAsync
    {
        private readonly ApplicationDbContext _context;

        public ProductImageRepositoryAsync(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<ProductImage?> GetProductImageByIdAndTenantIdAsync(
            Guid tenantId,
            Guid imageId,
            CancellationToken cancellationToken)
        {
            return await _context.ProductImages
                .FirstOrDefaultAsync(
                    x => x.TenantId == tenantId &&
                         x.Id == imageId,
                    cancellationToken);
        }

        public async Task<IReadOnlyList<ProductImage>> GetProductImagesByProductIdAsync(
            Guid tenantId,
            Guid productId,
            CancellationToken cancellationToken)
        {
            return await _context.ProductImages
                .AsNoTracking()
                .Where(x =>
                    x.TenantId == tenantId &&
                    x.ProductId == productId)
                .OrderByDescending(x => x.IsMain)
                .ThenBy(x => x.SortOrder)
                .ThenBy(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<ProductImage?> GetMainProductImageAsync(
            Guid tenantId,
            Guid productId,
            CancellationToken cancellationToken)
        {
            return await _context.ProductImages
                .FirstOrDefaultAsync(
                    x =>
                        x.TenantId == tenantId &&
                        x.ProductId == productId &&
                        x.IsMain,
                    cancellationToken);
        }

        public async Task<IReadOnlyList<ProductImage>> GetProductImagesForSetMainAsync(
            Guid tenantId,
            Guid productId,
            CancellationToken cancellationToken)
        {
            return await _context.ProductImages
                .Where(x =>
                    x.TenantId == tenantId &&
                    x.ProductId == productId)
                .ToListAsync(cancellationToken);
        }
    }
}
