using AutoMapper;
using MediatR;
using Pos.CatalogService.Application.Features.ProductVariants.DTOS;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;

namespace Pos.CatalogService.Application.Features.ProductVariants.Queries.GetByProductIdQuery
{
    public class GetProductVariantsQuery: IRequest<Result<IReadOnlyList<ProductVariantDto>>>
    {
        public Guid ProductId { get; set; }
    }

    public class GetProductVariantsQueryHandler: IRequestHandler<GetProductVariantsQuery, Result<IReadOnlyList<ProductVariantDto>>>
    {
        private readonly IProductRepositoryAsync _productRepository;
        private readonly IProductVariantRepositoryAsync _variantRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetProductVariantsQueryHandler(
            IProductRepositoryAsync productRepository,
            IProductVariantRepositoryAsync variantRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _variantRepository = variantRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<Result<IReadOnlyList<ProductVariantDto>>> Handle(
            GetProductVariantsQuery request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                return Result<IReadOnlyList<ProductVariantDto>>.Failure(
                    "TenantId claim is missing.");

            var product = await _productRepository.GetProductByIdAndTenantIdAsync(
                tenantId.Value,
                request.ProductId,
                includes: null,
                cancellationToken);

            if (product == null)
                return Result<IReadOnlyList<ProductVariantDto>>.Failure(
                    "Product is invalid.");

            var variants = await _variantRepository.GetProductVariantsByProductIdAsync(
                tenantId.Value,
                request.ProductId,
                cancellationToken);

            var dto = _mapper.Map<IReadOnlyList<ProductVariantDto>>(variants);

            return Result<IReadOnlyList<ProductVariantDto>>.Success(dto);
        }
    }
}
