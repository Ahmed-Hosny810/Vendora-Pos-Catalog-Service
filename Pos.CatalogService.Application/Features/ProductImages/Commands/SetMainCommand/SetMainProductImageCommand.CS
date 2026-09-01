using MediatR;
using Pos.CatalogService.Application.Interfaces;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;

namespace Pos.CatalogService.Application.Features.ProductImages.Commands.SetMainCommand
{
    public class SetMainProductImageCommand : IRequest<Result<Guid>>
    {
        public Guid ProductId { get; set; }

        public Guid ImageId { get; set; }
    }

    public class SetMainProductImageCommandHandler
        : IRequestHandler<SetMainProductImageCommand, Result<Guid>>
    {
        private readonly IProductRepositoryAsync _productRepository;
        private readonly IProductImageRepositoryAsync _imageRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public SetMainProductImageCommandHandler(
            IProductRepositoryAsync productRepository,
            IProductImageRepositoryAsync imageRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _imageRepository = imageRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(
            SetMainProductImageCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                return Result<Guid>.Failure("TenantId claim is missing.");

            var product = await _productRepository.GetProductByIdAndTenantIdForUpdateAsync(
                tenantId.Value,
                request.ProductId,
                cancellationToken);

            if (product == null)
                return Result<Guid>.Failure("Product is invalid.");

            var images = await _imageRepository.GetProductImagesForSetMainAsync(
                tenantId.Value,
                request.ProductId,
                cancellationToken);

            var selectedImage = images.FirstOrDefault(x => x.Id == request.ImageId);

            if (selectedImage == null)
                return Result<Guid>.Failure("Product image is invalid.");

            foreach (var image in images)
            {
                if (image.Id == selectedImage.Id)
                    image.MarkAsMain();
                else
                    image.UnmarkAsMain();

                _imageRepository.Update(image);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(selectedImage.Id);
        }
    }
}
