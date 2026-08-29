using AutoMapper;
using MediatR;
using Pos.CatalogService.Application.Features.Categories.DTOS;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.Categories.Queries.GetByIdQuery
{
    public class GetCategoryByIdQuery : IRequest<Result<CategoryDto>>
    {
        public Guid CategoryId { get; set; }
    }

    public class GetCategoryByIdQueryHandler
        : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
    {
        private readonly ICategoryRepositoryAsync _categoryRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetCategoryByIdQueryHandler(
            ICategoryRepositoryAsync categoryRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<Result<CategoryDto>> Handle(
            GetCategoryByIdQuery request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                return Result<CategoryDto>.Failure("TenantId claim is missing.");

            var category = await _categoryRepository.GetCategoryByIdAndTenantIdAsync(
                tenantId.Value,
                request.CategoryId,
                cancellationToken);

            if (category == null)
                return Result<CategoryDto>.Failure($"Category with Id {request.CategoryId} not found.");

            var dto = _mapper.Map<CategoryDto>(category);

            return Result<CategoryDto>.Success(dto);
        }
    }
}
