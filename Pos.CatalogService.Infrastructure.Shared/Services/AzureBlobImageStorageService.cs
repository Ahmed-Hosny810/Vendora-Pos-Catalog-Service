
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using Pos.CatalogService.Application.DTOS.Storage;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Infrastructure.Shared.Settings;

namespace Pos.CatalogService.Infrastructure.Shared.Services
{
    public class AzureBlobImageStorageService : IImageStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly AzureBlobStorageOptions _options;

        public AzureBlobImageStorageService(BlobServiceClient blobServiceClient,
            IOptions<AzureBlobStorageOptions> options)
        {

            _blobServiceClient = blobServiceClient;

            _options=options.Value;

        }
        public async Task<ImageUploadAccess> CreateUploadAccessAsync(string storageKey, TimeSpan expiresIn,
            CancellationToken cancellationToken)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_options.ProductImagesContainer);

            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(storageKey);

            var expiresAt = DateTimeOffset.UtcNow.Add(expiresIn);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerClient.Name,
                BlobName = storageKey,
                Resource = "b",
                ExpiresOn = expiresAt
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Create | BlobSasPermissions.Write);

            var uploadUri=blobClient.GenerateSasUri(sasBuilder);

            return new ImageUploadAccess(
            uploadUri.ToString(),
            expiresAt);
        }

        public async Task<StoredImageInfo?> GetBlobInfoAsync(string storageKey, CancellationToken cancellationToken)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_options.ProductImagesContainer);

            var blobClient= containerClient.GetBlobClient(storageKey);

            try
            {
                var response = await blobClient.GetPropertiesAsync();

                var properties = response.Value;

                return new StoredImageInfo(
                storageKey,
                blobClient.Uri.ToString(),
                properties.ContentLength,
                properties.ContentType,
                properties.LastModified);
            }
            catch (RequestFailedException ex)
            when (ex.Status == 404)
            {
                return null;
            }
        }
        public async Task DeleteAsync(string storageKey, CancellationToken cancellationToken)
        {
            var containerClient =
             _blobServiceClient.GetBlobContainerClient(
                 _options.ProductImagesContainer);

            var blobClient =
                containerClient.GetBlobClient(storageKey);

            await blobClient.DeleteIfExistsAsync(
                cancellationToken: cancellationToken);
        }

    }
}
