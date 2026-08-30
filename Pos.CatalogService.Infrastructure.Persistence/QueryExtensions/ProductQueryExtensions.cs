using Microsoft.EntityFrameworkCore;
using Pos.CatalogService.Application.Features.Products.Queries.GetAllQuery;
using Pos.CatalogService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Infrastructure.Persistence.QueryExtensions
{
    public static class ProductQueryExtensions
    {
        public static IQueryable<Product> ApplyFilters(
            this IQueryable<Product> query,
            Guid tenantId,
            ProductFilter? filter)
        {
            query = query.Where(x => x.TenantId == tenantId);

            if (filter == null)
                return query;

            if (filter.Id.HasValue)
                query = query.Where(x => x.Id == filter.Id.Value);

            if (filter.CategoryId.HasValue)
                query = query.Where(x => x.CategoryId == filter.CategoryId.Value);

            if (filter.UnitId.HasValue)
                query = query.Where(x => x.UnitId == filter.UnitId.Value);

            if (filter.TaxRateId.HasValue)
                query = query.Where(x => x.TaxRateId == filter.TaxRateId.Value);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim().ToLower();

                query = query.Where(x =>
                    x.NameEn.ToLower().Contains(search) ||
                    (x.NameAr != null && x.NameAr.ToLower().Contains(search)) ||
                    (x.Sku != null && x.Sku.ToLower().Contains(search)) ||
                    (x.Barcode != null && x.Barcode.ToLower().Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(filter.NameEn))
            {
                var nameEn = filter.NameEn.Trim().ToLower();

                query = query.Where(x =>
                    x.NameEn.ToLower().Contains(nameEn));
            }

            if (!string.IsNullOrWhiteSpace(filter.NameAr))
            {
                var nameAr = filter.NameAr.Trim().ToLower();

                query = query.Where(x =>
                    x.NameAr != null &&
                    x.NameAr.ToLower().Contains(nameAr));
            }

            if (!string.IsNullOrWhiteSpace(filter.Sku))
            {
                var sku = filter.Sku.Trim().ToLower();

                query = query.Where(x =>
                    x.Sku != null &&
                    x.Sku.ToLower().Contains(sku));
            }

            if (!string.IsNullOrWhiteSpace(filter.Barcode))
            {
                var barcode = filter.Barcode.Trim().ToLower();

                query = query.Where(x =>
                    x.Barcode != null &&
                    x.Barcode.ToLower().Contains(barcode));
            }

            if (filter.TrackInventory.HasValue)
            {
                query = query.Where(x =>
                    x.TrackInventory == filter.TrackInventory.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                var status = filter.Status.Trim();

                query = query.Where(x => x.Status == status);
            }

            return query;
        }

        public static IQueryable<Product> ApplyIncludes(
            this IQueryable<Product> query,
            ProductIncludes? includes)
        {
            if (includes == null)
                return query;

            if (includes.Category)
                query = query.Include(x => x.Category);

            if (includes.Unit)
                query = query.Include(x => x.Unit);

            if (includes.TaxRate)
                query = query.Include(x => x.TaxRate);

            if (includes.Images)
                query = query.Include(x => x.Images);

            if (includes.Variants)
                query = query.Include(x => x.Variants);

            return query;
        }

        public static IQueryable<Product> ApplyOrdering(
            this IQueryable<Product> query,
            ProductOrderKey orderKey,
            bool orderDescending)
        {
            return orderKey switch
            {
                ProductOrderKey.NameAr => orderDescending
                    ? query.OrderByDescending(x => x.NameAr)
                    : query.OrderBy(x => x.NameAr),

                ProductOrderKey.NameEn => orderDescending
                    ? query.OrderByDescending(x => x.NameEn)
                    : query.OrderBy(x => x.NameEn),

                ProductOrderKey.Sku => orderDescending
                    ? query.OrderByDescending(x => x.Sku)
                    : query.OrderBy(x => x.Sku),

                ProductOrderKey.Barcode => orderDescending
                    ? query.OrderByDescending(x => x.Barcode)
                    : query.OrderBy(x => x.Barcode),

                ProductOrderKey.CostPrice => orderDescending
                    ? query.OrderByDescending(x => x.CostPrice)
                    : query.OrderBy(x => x.CostPrice),

                ProductOrderKey.SellingPrice => orderDescending
                    ? query.OrderByDescending(x => x.SellingPrice)
                    : query.OrderBy(x => x.SellingPrice),

                ProductOrderKey.Status => orderDescending
                    ? query.OrderByDescending(x => x.Status)
                    : query.OrderBy(x => x.Status),

                ProductOrderKey.UpdatedAt => orderDescending
                    ? query.OrderByDescending(x => x.UpdatedAt)
                    : query.OrderBy(x => x.UpdatedAt),

                ProductOrderKey.CreatedAt => orderDescending
                    ? query.OrderByDescending(x => x.CreatedAt)
                    : query.OrderBy(x => x.CreatedAt),

                _ => query.OrderByDescending(x => x.CreatedAt)
            };
        }
    }
}
