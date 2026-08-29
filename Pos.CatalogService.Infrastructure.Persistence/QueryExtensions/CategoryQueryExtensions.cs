using Microsoft.EntityFrameworkCore;
using Pos.CatalogService.Application.Features.Categories.Queries.GetAllQuery;
using Pos.CatalogService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Infrastructure.Persistence.QueryExtensions
{
    public static class CategoryQueryExtensions
    {
        public static IQueryable<Category> ApplyFilters(
            this IQueryable<Category> query,
            Guid tenantId,
            CategoryFilter? filter)
        {
            query = query.Where(x => x.TenantId == tenantId);

            if (filter == null)
                return query;

            if (filter.Id.HasValue)
                query = query.Where(x => x.Id == filter.Id.Value);

            if (filter.ParentCategoryId.HasValue)
                query = query.Where(x => x.ParentCategoryId == filter.ParentCategoryId.Value);

            if (!string.IsNullOrWhiteSpace(filter.NameEn))
            {
                var nameEn = filter.NameEn.Trim().ToLower();

                query = query.Where(x => x.NameEn.ToLower().Contains(nameEn));
            }

            if (!string.IsNullOrWhiteSpace(filter.NameAr))
            {
                var nameAr = filter.NameAr.Trim().ToLower();

                query = query.Where(x =>
                    x.NameAr != null &&
                    x.NameAr.ToLower().Contains(nameAr));
            }

            if (filter.IsVisible.HasValue)
                query = query.Where(x => x.IsVisible == filter.IsVisible.Value);

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                var status = filter.Status.Trim();

                query = query.Where(x => x.Status == status);
            }

            return query;
        }

        public static IQueryable<Category> ApplyIncludes(
            this IQueryable<Category> query,
            CategoryIncludes? includes)
        {
            if (includes == null)
                return query;

            if (includes.ParentCategory)
                query = query.Include(x => x.ParentCategory);

            if (includes.Children)
                query = query.Include(x => x.Children);

            if (includes.Products)
                query = query.Include(x => x.Products);

            return query;
        }

        public static IQueryable<Category> ApplyOrdering(
            this IQueryable<Category> query,
            CategoryOrderKey orderKey,
            bool orderDescending)
        {
            return orderKey switch
            {
                CategoryOrderKey.NameAr => orderDescending
                    ? query.OrderByDescending(x => x.NameAr)
                    : query.OrderBy(x => x.NameAr),

                CategoryOrderKey.NameEn => orderDescending
                    ? query.OrderByDescending(x => x.NameEn)
                    : query.OrderBy(x => x.NameEn),

                CategoryOrderKey.Status => orderDescending
                    ? query.OrderByDescending(x => x.Status)
                    : query.OrderBy(x => x.Status),

                CategoryOrderKey.UpdatedAt => orderDescending
                    ? query.OrderByDescending(x => x.UpdatedAt)
                    : query.OrderBy(x => x.UpdatedAt),

                CategoryOrderKey.CreatedAt => orderDescending
                    ? query.OrderByDescending(x => x.CreatedAt)
                    : query.OrderBy(x => x.CreatedAt),

                CategoryOrderKey.SortOrder => orderDescending
                    ? query.OrderByDescending(x => x.SortOrder)
                    : query.OrderBy(x => x.SortOrder),

                _ => query.OrderBy(x => x.SortOrder)
                          .ThenBy(x => x.NameEn)
            };
        }
    }
}
