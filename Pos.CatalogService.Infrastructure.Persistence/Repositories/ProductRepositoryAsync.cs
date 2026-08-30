using Microsoft.EntityFrameworkCore;
using Pos.CatalogService.Application.Features.Products.Queries.GetAllQuery;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Wrappers;
using Pos.CatalogService.Domain.Models;
using Pos.CatalogService.Infrastructure.Persistence.Contexts;
using Pos.CatalogService.Infrastructure.Persistence.QueryExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Infrastructure.Persistence.Repositories
{
    public class ProductRepositoryAsync: GenericRepositoryAsync<Product, Guid>, IProductRepositoryAsync
    {
        private readonly ApplicationDbContext _context;

        public ProductRepositoryAsync(ApplicationDbContext context)
            :base(context)
        {
            _context = context;
        }

        public async Task<bool> IsSkuExistsAsync(
            Guid tenantId,
            string sku,
            CancellationToken cancellationToken)
        {
            var normalizedSku = sku.Trim();

            return await _context.Products
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

            return await _context.Products
                .AsNoTracking()
                .AnyAsync(
                    x => x.TenantId == tenantId &&
                         x.Barcode == normalizedBarcode,
                    cancellationToken);
        }

        public async Task<bool> IsSkuExistsForAnotherProductAsync(
            Guid tenantId,
            Guid productId,
            string sku,
            CancellationToken cancellationToken)
        {
            var normalizedSku = sku.Trim();

            return await _context.Products
                .AsNoTracking()
                .AnyAsync(
                    x => x.TenantId == tenantId &&
                         x.Id != productId &&
                         x.Sku == normalizedSku,
                    cancellationToken);
        }

        public async Task<bool> IsBarcodeExistsForAnotherProductAsync(
            Guid tenantId,
            Guid productId,
            string barcode,
            CancellationToken cancellationToken)
        {
            var normalizedBarcode = barcode.Trim();

            return await _context.Products
                .AsNoTracking()
                .AnyAsync(
                    x => x.TenantId == tenantId &&
                         x.Id != productId &&
                         x.Barcode == normalizedBarcode,
                    cancellationToken);
        }

        public async Task<Product?> GetProductByIdAndTenantIdForUpdateAsync(Guid tenantId,Guid productId,CancellationToken cancellationToken)
        {
            return await _context.Products
                .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == productId,
                    cancellationToken);
        }


        public async Task<Product?> GetProductByIdAndTenantIdAsync(Guid tenantId,Guid productId,ProductIncludes? includes,
            CancellationToken cancellationToken)
        {
            return await _context.Products
                .AsNoTracking()
                .ApplyIncludes(includes)
                .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == productId,
                    cancellationToken);
        }

        public async Task<PagedResponse<IEnumerable<Product>>> GetProductsPagedResponseAsync(Guid tenantId, ProductFilter? filter, ProductIncludes? includes, ProductOrderKey orderKey, bool orderDescending, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var query = _context.Products.AsNoTracking();

            var filteredQuery = query.ApplyFilters(
                tenantId,
                filter);

            var totalRecords = await filteredQuery
                .CountAsync(cancellationToken);

            var products = await filteredQuery
                .ApplyIncludes(includes)
                .ApplyOrdering(orderKey, orderDescending)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResponse<IEnumerable<Product>>(
                products,
                pageNumber,
                pageSize,
                totalRecords);
        }
    }
}
