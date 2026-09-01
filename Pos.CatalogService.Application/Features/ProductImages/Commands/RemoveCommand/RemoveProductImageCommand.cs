using MediatR;
using Pos.CatalogService.Application.Interfaces;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;

namespace Pos.CatalogService.Application.Features.ProductImages.Commands.RemoveCommand
{
    public class RemoveProductImageCommand : IRequest<Result>
    {
        public Guid ImageId { get; set; }
    }

    public class RemoveProductImageCommandHandler
        : IRequestHandler<RemoveProductImageCommand, Result>
    {
        private readonly IProductImageRepositoryAsync _imageRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveProductImageCommandHandler(
            IProductImageRepositoryAsync imageRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _imageRepository = imageRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(
            RemoveProductImageCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                return Result.Failure("TenantId claim is missing.");

            var image = await _imageRepository.GetProductImageByIdAndTenantIdAsync(
                tenantId.Value,
                request.ImageId,
                cancellationToken);

            if (image == null)
                return Result.Failure($"Product image with Id {request.ImageId} not found.");

            _imageRepository.Delete(image);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
