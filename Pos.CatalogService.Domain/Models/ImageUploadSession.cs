
using Pos.CatalogService.Domain.Constants;

namespace Pos.CatalogService.Domain.Models
{
    public class ImageUploadSession
    {
        public Guid Id { get;  set; }

        public Guid TenantId { get;  set; }

        public Guid ProductId { get;  set; }

        public string StorageKey { get;  set; } = null!;

        public string ExpectedContentType { get;  set; } = null!;

        public long MaxSizeBytes { get;  set; }

        public string Status { get;  set; } = ImageUploadStatuses.Pending;

        public DateTimeOffset ExpiresAt { get;  set; }

        public DateTimeOffset CreatedAt { get;  set; }


        public void Complete()
        {
            Status = ImageUploadStatuses.Completed;
        }

        public void Reject()
        {
            Status = ImageUploadStatuses.Rejected;
        }

        public void Expire()
        {
            Status = ImageUploadStatuses.Expired;
        }
    }
}
