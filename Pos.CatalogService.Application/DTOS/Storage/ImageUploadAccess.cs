
namespace Pos.CatalogService.Application.DTOS.Storage
{
    public sealed record ImageUploadAccess(
    string UploadUrl,
    DateTimeOffset ExpiresAt);
}
