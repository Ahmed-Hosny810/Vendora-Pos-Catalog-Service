using Pos.CatalogService.Application.DTOS.Storage;
using Pos.CatalogService.Application.Interfaces.Services;

namespace Pos.CatalogService.IntegrationTests.Fakes
{
    public class FakeImageStorageService : IImageStorageService
    {
        public StoredImageInfo? BlobInfo { get; set; }

        public bool ThrowWhenCreatingUploadAccess { get; set; }

        public List<string> DeletedStorageKeys { get; } = new();

        public Task<ImageUploadAccess> CreateUploadAccessAsync(
            string storageKey,
            TimeSpan expiresIn,
            CancellationToken cancellationToken)
        {
            if (ThrowWhenCreatingUploadAccess)
            {
                throw new InvalidOperationException(
                    "Fake storage service failure.");
            }

            return Task.FromResult(
                new ImageUploadAccess(
                    $"https://fake-storage.test/upload/{storageKey}",
                    DateTimeOffset.UtcNow.Add(expiresIn)));
        }

        public Task<StoredImageInfo?> GetBlobInfoAsync(
            string storageKey,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(BlobInfo);
        }

        public Task DeleteAsync(
            string storageKey,
            CancellationToken cancellationToken)
        {
            DeletedStorageKeys.Add(storageKey);

            return Task.CompletedTask;
        }
    }
}
