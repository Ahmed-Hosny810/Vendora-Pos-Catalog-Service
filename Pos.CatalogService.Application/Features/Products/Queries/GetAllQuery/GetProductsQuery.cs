using AutoMapper;
using MediatR;
using Pos.CatalogService.Application.Features.Products.DTOS;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.Products.Queries.GetAllQuery
{
    public class GetProductsQuery
       : IRequest<PagedResponse<IEnumerable<ProductDto>>>
    {
        public GetProductsQueryParameter Parameter { get; set; } = new();
    }

    public class GetProductsQueryHandler: IRequestHandler<GetProductsQuery, PagedResponse<IEnumerable<ProductDto>>>
    {
        private readonly IProductRepositoryAsync _productRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetProductsQueryHandler(
            IProductRepositoryAsync productRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<PagedResponse<IEnumerable<ProductDto>>> Handle(
            GetProductsQuery request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
            {
                return new PagedResponse<IEnumerable<ProductDto>>(
                    Enumerable.Empty<ProductDto>(),
                    request.Parameter.PageNumber,
                    request.Parameter.PageSize,
                    0);
            }

            var includes = request.Parameter.Includes ?? new ProductIncludes
            {
                Category = true,
                Unit = true,
                TaxRate = true,
                Images = false,
                Variants = false
            };

            var pagedProducts = await _productRepository.GetProductsPagedResponseAsync(
                tenantId.Value,
                request.Parameter.Filter,
                includes,
                request.Parameter.OrderKey,
                request.Parameter.OrderDescending,
                request.Parameter.PageNumber,
                request.Parameter.PageSize,
                cancellationToken);

            var dto = _mapper.Map<IEnumerable<ProductDto>>(pagedProducts.Data);

            return new PagedResponse<IEnumerable<ProductDto>>(
                dto,
                pagedProducts.PageNumber,
                pagedProducts.PageSize,
                pagedProducts.TotalCount);
        }
    }
}
