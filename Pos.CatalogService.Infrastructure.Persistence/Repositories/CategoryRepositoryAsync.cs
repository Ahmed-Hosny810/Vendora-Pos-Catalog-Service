using Microsoft.EntityFrameworkCore;
using Pos.CatalogService.Application.Features.Categories.Queries.GetAllQuery;
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
    public class CategoryRepositoryAsync
        : GenericRepositoryAsync<Category, Guid>, ICategoryRepositoryAsync
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepositoryAsync(ApplicationDbContext context,CancellationToken cancellationToken)
            : base(context, cancellationToken)
        {
            _context = context;
        }

        public async Task<Category?> GetCategoryByIdAndTenantIdAsync(
            Guid tenantId,
            Guid categoryId,
            CancellationToken cancellationToken)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(
                    x => x.TenantId == tenantId && x.Id == categoryId,
                    cancellationToken);
        }

        public async Task<bool> IsCategoryNameExistsAsync(
            Guid tenantId,
            string nameEn,
            CancellationToken cancellationToken)
        {
            var normalizedName = nameEn.Trim();

            return await _context.Categories
                .AsNoTracking()
                .AnyAsync(
                    x => x.TenantId == tenantId &&
                         x.NameEn == normalizedName,
                    cancellationToken);
        }

        public async Task<bool> IsCategoryNameExistsForAnotherCategoryAsync(
            Guid tenantId,
            Guid categoryId,
            string nameEn,
            CancellationToken cancellationToken)
        {
            var normalizedName = nameEn.Trim();

            return await _context.Categories
                .AsNoTracking()
                .AnyAsync(
                    x => x.TenantId == tenantId &&
                         x.Id != categoryId &&
                         x.NameEn == normalizedName,
                    cancellationToken);
        }

        public async Task<bool> IsParentCategoryValidAsync(
            Guid tenantId,
            Guid parentCategoryId,
            CancellationToken cancellationToken)
        {
            return await _context.Categories
                .AsNoTracking()
                .AnyAsync(
                    x => x.TenantId == tenantId &&
                         x.Id == parentCategoryId,
                    cancellationToken);
        }

        public async Task<PagedResponse<IEnumerable<Category>>> GetCategoriesPagedResponseAsync(
            Guid tenantId,
            CategoryFilter? filter,
            CategoryIncludes? includes,
            CategoryOrderKey orderKey,
            bool orderDescending,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var query = _context.Categories.AsNoTracking();

            var filteredQuery = query.ApplyFilters(tenantId, filter);

            var totalRecords = await filteredQuery
                .CountAsync(cancellationToken);

            var categories = await filteredQuery
                .ApplyIncludes(includes)
                .ApplyOrdering(orderKey, orderDescending)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResponse<IEnumerable<Category>>(
                categories,
                pageNumber,
                pageSize,
                totalRecords);
        }
    }
}
