

namespace Pos.CatalogService.Application.Features.ProductImages.Commands.StartUploadCommand
{
    public sealed record StartProductImageUploadResponse(
    Guid UploadSessionId,
    string UploadUrl,
    DateTimeOffset ExpiresAt);
}
