using Pos.CatalogService.Application.Features.Categories.Queries.GetAllQuery;
using Pos.CatalogService.Application.Wrappers;
using Pos.CatalogService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Interfaces.Repositories
{
    public interface ICategoryRepositoryAsync : IGenericRepositoryAsync<Category, Guid>
    {
        Task<Category?> GetCategoryByIdAndTenantIdAsync(
            Guid tenantId,
            Guid categoryId,
            CancellationToken cancellationToken);

        Task<bool> IsCategoryNameExistsAsync(
            Guid tenantId,
            string nameEn,
            CancellationToken cancellationToken);

        Task<bool> IsCategoryNameExistsForAnotherCategoryAsync(
            Guid tenantId,
            Guid categoryId,
            string nameEn,
            CancellationToken cancellationToken);

        Task<bool> IsParentCategoryValidAsync(
            Guid tenantId,
            Guid parentCategoryId,
            CancellationToken cancellationToken);

        Task<PagedResponse<IEnumerable<Category>>> GetCategoriesPagedResponseAsync(
            Guid tenantId,
            CategoryFilter? filter,
            CategoryIncludes? includes,
            CategoryOrderKey orderKey,
            bool orderDescending,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken);
    }
}
