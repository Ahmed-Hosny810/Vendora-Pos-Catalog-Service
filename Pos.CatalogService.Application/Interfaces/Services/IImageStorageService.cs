
using Pos.CatalogService.Application.DTOS.Storage;

namespace Pos.CatalogService.Application.Interfaces.Services
{
    public interface IImageStorageService
    {
        Task<ImageUploadAccess> CreateUploadAccessAsync(
        string storageKey,
        TimeSpan expiresIn,
        CancellationToken cancellationToken);

        Task<StoredImageInfo?> GetBlobInfoAsync(
            string storageKey,
            CancellationToken cancellationToken);

        Task DeleteAsync(
            string storageKey,
            CancellationToken cancellationToken);
    }
}
