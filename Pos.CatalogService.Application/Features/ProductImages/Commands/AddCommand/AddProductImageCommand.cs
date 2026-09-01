using MediatR;
using Pos.CatalogService.Application.Interfaces;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;
using Pos.CatalogService.Domain.Models;


namespace Pos.CatalogService.Application.Features.ProductImages.Commands.AddCommand
{
    public class AddProductImageCommand : IRequest<Result<Guid>>
    {
        public Guid ProductId { get; set; }

        public string ImageUrl { get; set; } = null!;

        public int SortOrder { get; set; }

        public bool IsMain { get; set; }
    }

    public class AddProductImageCommandHandler
        : IRequestHandler<AddProductImageCommand, Result<Guid>>
    {
        private readonly IProductRepositoryAsync _productRepository;
        private readonly IProductImageRepositoryAsync _imageRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public AddProductImageCommandHandler(
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
            AddProductImageCommand request,
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

            if (request.IsMain)
            {
                var currentMainImage = await _imageRepository.GetMainProductImageAsync(
                    tenantId.Value,
                    request.ProductId,
                    cancellationToken);

                if (currentMainImage != null)
                {
                    currentMainImage.UnmarkAsMain();
                    _imageRepository.Update(currentMainImage);
                }
            }

            var image = new ProductImage
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId.Value,
                ProductId = product.Id,
                ImageUrl = request.ImageUrl.Trim(),
                SortOrder = request.SortOrder,
                IsMain = request.IsMain,
                CreatedAt = DateTime.UtcNow
            };

            await _imageRepository.AddAsync(image, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(image.Id);
        }
    }
}
