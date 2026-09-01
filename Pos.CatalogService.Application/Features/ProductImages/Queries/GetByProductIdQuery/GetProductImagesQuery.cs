using AutoMapper;
using MediatR;
using Pos.CatalogService.Application.Features.ProductImages.DTOS;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;


namespace Pos.CatalogService.Application.Features.ProductImages.Queries.GetByProductIdQuery
{
    public class GetProductImagesQuery
        : IRequest<Result<IReadOnlyList<ProductImageDto>>>
    {
        public Guid ProductId { get; set; }
    }

    public class GetProductImagesQueryHandler
        : IRequestHandler<GetProductImagesQuery, Result<IReadOnlyList<ProductImageDto>>>
    {
        private readonly IProductRepositoryAsync _productRepository;
        private readonly IProductImageRepositoryAsync _imageRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetProductImagesQueryHandler(
            IProductRepositoryAsync productRepository,
            IProductImageRepositoryAsync imageRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _imageRepository = imageRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<Result<IReadOnlyList<ProductImageDto>>> Handle(
            GetProductImagesQuery request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                return Result<IReadOnlyList<ProductImageDto>>.Failure(
                    "TenantId claim is missing.");

            var product = await _productRepository.GetProductByIdAndTenantIdAsync(
                tenantId.Value,
                request.ProductId,
                includes: null,
                cancellationToken);

            if (product == null)
                return Result<IReadOnlyList<ProductImageDto>>.Failure(
                    "Product is invalid.");

            var images = await _imageRepository.GetProductImagesByProductIdAsync(
                tenantId.Value,
                request.ProductId,
                cancellationToken);

            var dto = _mapper.Map<IReadOnlyList<ProductImageDto>>(images);

            return Result<IReadOnlyList<ProductImageDto>>.Success(dto);
        }
    }
}
