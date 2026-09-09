

namespace Pos.CatalogService.Application.Settings
{
    public sealed class ProductImageUploadSettings
    {
        public const string SectionName = "ProductImageUpload";

        public long MaxSizeBytes { get; set; }

        public int UploadExpirationMinutes { get; set; }

        public string[] AllowedContentTypes { get; set; } = [];

        public string[] AllowedExtensions { get; set; } = [];
    }
}
