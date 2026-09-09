
namespace Pos.CatalogService.Application.DTOS.Storage
{
    public sealed record StoredImageInfo(
    string StorageKey,
    string Url,
    long Size,
    string? ContentType,
    DateTimeOffset? LastModified);
}
