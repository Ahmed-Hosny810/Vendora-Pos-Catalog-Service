using Pos.CatalogService.Application.Features.Units.Queries.GetAllQuery;
using Pos.CatalogService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Infrastructure.Persistence.QueryExtensions
{
    public static class UnitQueryExtensions
    {
        public static IQueryable<Unit> ApplyFilters(
            this IQueryable<Unit> query,
            Guid tenantId,
            UnitFilter? filter)
        {
            query = query.Where(x =>
                x.TenantId == null ||
                x.TenantId == tenantId);

            if (filter == null)
                return query;

            if (filter.Id.HasValue)
                query = query.Where(x => x.Id == filter.Id.Value);

            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                var name = filter.Name.Trim().ToLower();

                query = query.Where(x =>
                    x.Name.ToLower().Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(filter.Symbol))
            {
                var symbol = filter.Symbol.Trim().ToLower();

                query = query.Where(x =>
                    x.Symbol.ToLower().Contains(symbol));
            }

            if (filter.IsDecimalAllowed.HasValue)
                query = query.Where(x => x.IsDecimalAllowed == filter.IsDecimalAllowed.Value);

            if (filter.IsGlobal.HasValue)
            {
                query = filter.IsGlobal.Value
                    ? query.Where(x => x.TenantId == null)
                    : query.Where(x => x.TenantId == tenantId);
            }

            return query;
        }

        public static IQueryable<Unit> ApplyOrdering(
            this IQueryable<Unit> query,
            UnitOrderKey orderKey,
            bool orderDescending)
        {
            return orderKey switch
            {
                UnitOrderKey.Symbol => orderDescending
                    ? query.OrderByDescending(x => x.Symbol)
                    : query.OrderBy(x => x.Symbol),

                UnitOrderKey.CreatedAt => orderDescending
                    ? query.OrderByDescending(x => x.CreatedAt)
                    : query.OrderBy(x => x.CreatedAt),

                UnitOrderKey.UpdatedAt => orderDescending
                    ? query.OrderByDescending(x => x.UpdatedAt)
                    : query.OrderBy(x => x.UpdatedAt),

                UnitOrderKey.Name => orderDescending
                    ? query.OrderByDescending(x => x.Name)
                    : query.OrderBy(x => x.Name),

                _ => query.OrderBy(x => x.Name)
            };
        }
    }
}
