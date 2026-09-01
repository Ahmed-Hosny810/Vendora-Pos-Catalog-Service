using Microsoft.EntityFrameworkCore;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Domain.Models;
using Pos.CatalogService.Infrastructure.Persistence.Contexts;

namespace Pos.CatalogService.Infrastructure.Persistence.Repositories
{
    public class ProductVariantRepositoryAsync
        : GenericRepositoryAsync<ProductVariant, Guid>, IProductVariantRepositoryAsync
    {
        private readonly ApplicationDbContext _context;

        public ProductVariantRepositoryAsync(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<ProductVariant?> GetProductVariantByIdAndTenantIdAsync(
            Guid tenantId,
            Guid variantId,
            CancellationToken cancellationToken)
        {
            return await _context.ProductVariants
                .FirstOrDefaultAsync(
                    x => x.TenantId == tenantId &&
                         x.Id == variantId,
                    cancellationToken);
        }

        public async Task<IReadOnlyList<ProductVariant>> GetProductVariantsByProductIdAsync(
            Guid tenantId,
            Guid productId,
            CancellationToken cancellationToken)
        {
            return await _context.ProductVariants
                .AsNoTracking()
                .Where(x =>
                    x.TenantId == tenantId &&
                    x.ProductId == productId)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsSkuExistsAsync(
            Guid tenantId,
            string sku,
            CancellationToken cancellationToken)
        {
            var normalizedSku = sku.Trim();

            return await _context.ProductVariants
                .AsNoTracking()
                .AnyAsync(
                    x => x.TenantId == tenantId &&
                         x.Sku == normalizedSku,
                    cancellationToken);
        }

        public async Task<bool> IsBarcodeExistsAsync(
            Guid tenantId,
            string barcode,
            CancellationToken cancellationToken)
        {
            var normalizedBarcode = barcode.Trim();

            return await _context.ProductVariants
                .AsNoTracking()
                .AnyAsync(
                    x => x.TenantId == tenantId &&
                         x.Barcode == normalizedBarcode,
                    cancellationToken);
        }

        public async Task<bool> IsSkuExistsForAnotherVariantAsync(
            Guid tenantId,
            Guid variantId,
            string sku,
            CancellationToken cancellationToken)
        {
            var normalizedSku = sku.Trim();

            return await _context.ProductVariants
                .AsNoTracking()
                .AnyAsync(
                    x => x.TenantId == tenantId &&
                         x.Id != variantId &&
                         x.Sku == normalizedSku,
                    cancellationToken);
        }

        public async Task<bool> IsBarcodeExistsForAnotherVariantAsync(
            Guid tenantId,
            Guid variantId,
            string barcode,
            CancellationToken cancellationToken)
        {
            var normalizedBarcode = barcode.Trim();

            return await _context.ProductVariants
                .AsNoTracking()
                .AnyAsync(
                    x => x.TenantId == tenantId &&
                         x.Id != variantId &&
                         x.Barcode == normalizedBarcode,
                    cancellationToken);
        }
    }
}
