using Microsoft.EntityFrameworkCore;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Domain.Models;
using Pos.CatalogService.Infrastructure.Persistence.Contexts;


namespace Pos.CatalogService.Infrastructure.Persistence.Repositories
{
    public class TaxRateRepositoryAsync
       : GenericRepositoryAsync<TaxRate, Guid>, ITaxRateRepositoryAsync
    {
        private readonly ApplicationDbContext _context;

        public TaxRateRepositoryAsync(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<TaxRate>> GetTaxRatesByTenantIdAsync(
            Guid tenantId,
            CancellationToken cancellationToken)
        {
            return await _context.TaxRates
                .AsNoTracking()
                .Where(x => x.TenantId == tenantId)
                .OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.IsActive)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<TaxRate?> GetTaxRateByIdAndTenantIdAsync(
            Guid tenantId,
            Guid taxRateId,
            CancellationToken cancellationToken)
        {
            return await _context.TaxRates
                .FirstOrDefaultAsync(
                    x => x.TenantId == tenantId && x.Id == taxRateId,
                    cancellationToken);
        }

        public async Task<TaxRate?> GetDefaultTaxRateAsync(
            Guid tenantId,
            CancellationToken cancellationToken)
        {
            return await _context.TaxRates
                .FirstOrDefaultAsync(
                    x => x.TenantId == tenantId && x.IsDefault,
                    cancellationToken);
        }

        public async Task<TaxRate?> GetTaxRateByNameAsync(
            Guid tenantId,
            string name,
            CancellationToken cancellationToken)
        {
            var normalizedName = name.Trim();

            return await _context.TaxRates
                .FirstOrDefaultAsync(
                    x => x.TenantId == tenantId && x.Name == normalizedName,
                    cancellationToken);
        }

        public async Task<bool> IsTaxRateNameExistsAsync(
            Guid tenantId,
            string name,
            CancellationToken cancellationToken)
        {
            var normalizedName = name.Trim();

            return await _context.TaxRates
                .AsNoTracking()
                .AnyAsync(
                    x => x.TenantId == tenantId && x.Name == normalizedName,
                    cancellationToken);
        }
    }
}
