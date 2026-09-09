using Pos.CatalogService.Domain.Models;


namespace Pos.CatalogService.Application.Interfaces.Repositories
{
    public interface IImageUploadSessionRepositoryAsync:IGenericRepositoryAsync<ImageUploadSession,Guid>
    {
        Task<ImageUploadSession?> GetByIdAndTenantIdAsync(
        Guid id,
        Guid tenantId,
        CancellationToken cancellationToken);
    }
}
