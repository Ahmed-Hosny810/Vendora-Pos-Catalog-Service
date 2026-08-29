using Pos.CatalogService.Domain.Models;


namespace Pos.CatalogService.Application.Interfaces.Repositories
{
    public interface ITaxRateRepositoryAsync : IGenericRepositoryAsync<TaxRate, Guid>
    {
        Task<IReadOnlyList<TaxRate>> GetTaxRatesByTenantIdAsync(
            Guid tenantId,
            CancellationToken cancellationToken);

        Task<TaxRate?> GetTaxRateByIdAndTenantIdAsync(
            Guid tenantId,
            Guid taxRateId,
            CancellationToken cancellationToken);

        Task<TaxRate?> GetDefaultTaxRateAsync(
            Guid tenantId,
            CancellationToken cancellationToken);

        Task<TaxRate?> GetTaxRateByNameAsync(
            Guid tenantId,
            string name,
            CancellationToken cancellationToken);

        Task<bool> IsTaxRateNameExistsAsync(
            Guid tenantId,
            string name,
            CancellationToken cancellationToken);
    }
}
