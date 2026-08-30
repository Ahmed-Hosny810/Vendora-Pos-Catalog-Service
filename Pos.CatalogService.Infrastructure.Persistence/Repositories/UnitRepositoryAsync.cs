using Microsoft.EntityFrameworkCore;
using Pos.CatalogService.Application.Features.Units.Queries.GetAllQuery;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Wrappers;
using Pos.CatalogService.Domain.Models;
using Pos.CatalogService.Infrastructure.Persistence.Contexts;
using Pos.CatalogService.Infrastructure.Persistence.QueryExtensions;


namespace Pos.CatalogService.Infrastructure.Persistence.Repositories
{
    public class UnitRepositoryAsync
        : GenericRepositoryAsync<Unit, Guid>, IUnitRepositoryAsync
    {
        private readonly ApplicationDbContext _context;

        public UnitRepositoryAsync(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<Unit?> GetUnitByIdForTenantAsync(
            Guid tenantId,
            Guid unitId,
            CancellationToken cancellationToken)
        {
            return await _context.Units
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>x.Id == unitId &&(x.TenantId == null || x.TenantId == tenantId),cancellationToken);
        }

        public async Task<Unit?> GetTenantUnitByIdAsync(
            Guid tenantId,
            Guid unitId,
            CancellationToken cancellationToken)
        {
            return await _context.Units
                .FirstOrDefaultAsync(
                    x => x.Id == unitId && x.TenantId == tenantId,
                    cancellationToken);
        }

        public async Task<bool> IsSymbolExistsInTenantScopeAsync(
            Guid tenantId,
            string symbol,
            CancellationToken cancellationToken)
        {
            var normalizedSymbol = symbol.Trim();

            return await _context.Units
                .AsNoTracking()
                .AnyAsync(
                    x =>(x.TenantId == null || x.TenantId == tenantId) &&x.Symbol == normalizedSymbol,
                    cancellationToken);
        }

        public async Task<bool> IsSymbolExistsForAnotherTenantUnitAsync(
            Guid tenantId,
            Guid unitId,
            string symbol,
            CancellationToken cancellationToken)
        {
            var normalizedSymbol = symbol.Trim();

            return await _context.Units
                .AsNoTracking()
                .AnyAsync(
                    x =>x.Id != unitId &&(x.TenantId == null || x.TenantId == tenantId) &&x.Symbol == normalizedSymbol,
                    cancellationToken);
        }

        public async Task<PagedResponse<IEnumerable<Unit>>> GetUnitsPagedResponseAsync(
            Guid tenantId,
            UnitFilter? filter,
            UnitOrderKey orderKey,
            bool orderDescending,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;  

            var query = _context.Units.AsNoTracking();

            var filteredQuery = query.ApplyFilters(tenantId, filter);

            var totalRecords = await filteredQuery
                .CountAsync(cancellationToken);

            var units = await filteredQuery
                .ApplyOrdering(orderKey, orderDescending)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResponse<IEnumerable<Unit>>(
                units,
                pageNumber,
                pageSize,
                totalRecords);
        }
    }
}
