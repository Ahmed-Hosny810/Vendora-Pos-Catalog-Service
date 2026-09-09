using Microsoft.EntityFrameworkCore;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Domain.Models;
using Pos.CatalogService.Infrastructure.Persistence.Contexts;

namespace Pos.CatalogService.Infrastructure.Persistence.Repositories
{
    public class ImageUploadSessionRepositoryAsync : GenericRepositoryAsync<ImageUploadSession, Guid>, IImageUploadSessionRepositoryAsync
    {
        private readonly ApplicationDbContext _context;

        public ImageUploadSessionRepositoryAsync(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ImageUploadSession?> GetByIdAndTenantIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken)
        {
            return await _context.ImageUploadSessions
                .FirstOrDefaultAsync(u => u.Id == id && u.TenantId == tenantId);
        }
    }
}
