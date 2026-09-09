
namespace Pos.CatalogService.Infrastructure.Shared.Settings
{
    public sealed class AzureBlobStorageOptions
    {
        public const string SectionName = "AzureBlobStorage";

        public string ProductImagesContainer { get; set; } = "product-images";
    }
}
