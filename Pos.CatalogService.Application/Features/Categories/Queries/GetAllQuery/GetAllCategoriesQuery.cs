using AutoMapper;
using MediatR;
using Pos.CatalogService.Application.Features.Categories.DTOS;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.Categories.Queries.GetAllQuery
{
    public class GetAllCategoriesQuery
        : IRequest<PagedResponse<IEnumerable<CategoryDto>>>
    {
        public GetAllCategoriesQueryParameter Parameter { get; set; } = new();
    }

    public class GetAllCategoriesQueryHandler
        : IRequestHandler<GetAllCategoriesQuery, PagedResponse<IEnumerable<CategoryDto>>>
    {
        private readonly ICategoryRepositoryAsync _categoryRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetAllCategoriesQueryHandler(
            ICategoryRepositoryAsync categoryRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<PagedResponse<IEnumerable<CategoryDto>>> Handle(
            GetAllCategoriesQuery request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
            {
                return new PagedResponse<IEnumerable<CategoryDto>>(
                    Enumerable.Empty<CategoryDto>(),
                    request.Parameter.PageNumber,
                    request.Parameter.PageSize,
                    0);
            }

            var pagedCategories = await _categoryRepository.GetCategoriesPagedResponseAsync(
                tenantId.Value,
                request.Parameter.Filter,
                request.Parameter.Includes,
                request.Parameter.OrderKey,
                request.Parameter.OrderDescending,
                request.Parameter.PageNumber,
                request.Parameter.PageSize,
                cancellationToken);

            var dto = _mapper.Map<IEnumerable<CategoryDto>>(pagedCategories.Data);

            return new PagedResponse<IEnumerable<CategoryDto>>(
                dto,
                pagedCategories.PageNumber,
                pagedCategories.PageSize,
                pagedCategories.TotalCount);
        }
    }
}
