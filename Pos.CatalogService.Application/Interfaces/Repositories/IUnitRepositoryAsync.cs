using Pos.CatalogService.Application.Features.Units.Queries.GetAllQuery;
using Pos.CatalogService.Application.Wrappers;
using Pos.CatalogService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Interfaces.Repositories
{
    public interface IUnitRepositoryAsync : IGenericRepositoryAsync<Unit, Guid>
    {
        Task<Unit?> GetUnitByIdForTenantAsync(
            Guid tenantId,
            Guid unitId,
            CancellationToken cancellationToken);

        Task<Unit?> GetTenantUnitByIdAsync(
            Guid tenantId,
            Guid unitId,
            CancellationToken cancellationToken);

        Task<bool> IsSymbolExistsInTenantScopeAsync(
            Guid tenantId,
            string symbol,
            CancellationToken cancellationToken);

        Task<bool> IsSymbolExistsForAnotherTenantUnitAsync(
            Guid tenantId,
            Guid unitId,
            string symbol,
            CancellationToken cancellationToken);

        Task<PagedResponse<IEnumerable<Unit>>> GetUnitsPagedResponseAsync(
            Guid tenantId,
            UnitFilter? filter,
            UnitOrderKey orderKey,
            bool orderDescending,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken);
    }
}
