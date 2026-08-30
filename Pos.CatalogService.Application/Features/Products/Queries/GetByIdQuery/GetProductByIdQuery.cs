using AutoMapper;
using MediatR;
using Pos.CatalogService.Application.Features.Products.DTOS;
using Pos.CatalogService.Application.Features.Products.Queries.GetAllQuery;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.Products.Queries.GetByIdQuery
{
    public class GetProductByIdQuery : IRequest<Result<ProductDto>>
    {
        public Guid ProductId { get; set; }

        public ProductIncludes? Includes { get; set; }
    }

    public class GetProductByIdQueryHandler
        : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
    {
        private readonly IProductRepositoryAsync _productRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetProductByIdQueryHandler(
            IProductRepositoryAsync productRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<Result<ProductDto>> Handle(
            GetProductByIdQuery request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                return Result<ProductDto>.Failure("TenantId claim is missing.");

            var includes = request.Includes ?? new ProductIncludes
            {
                Category = true,
                Unit = true,
                TaxRate = true,
                Images = true,
                Variants = true
            };

            var product = await _productRepository.GetProductByIdAndTenantIdAsync(
                tenantId.Value,
                request.ProductId,
                includes,
                cancellationToken);

            if (product == null)
                return Result<ProductDto>.Failure(
                    $"Product with Id {request.ProductId} not found.");

            var dto = _mapper.Map<ProductDto>(product);

            return Result<ProductDto>.Success(dto);
        }
    }

}
