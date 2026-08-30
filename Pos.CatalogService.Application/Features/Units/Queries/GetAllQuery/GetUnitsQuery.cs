using AutoMapper;
using MediatR;
using Pos.CatalogService.Application.Features.Units.DTOS;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.Units.Queries.GetAllQuery
{
    public class GetUnitsQuery : IRequest<PagedResponse<IEnumerable<UnitDto>>>
    {
        public GetUnitsQueryParameter Parameter { get; set; } = new();
    }

    public class GetUnitsQueryHandler : IRequestHandler<GetUnitsQuery, PagedResponse<IEnumerable<UnitDto>>>
    {
        private readonly IUnitRepositoryAsync _unitRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetUnitsQueryHandler(
            IUnitRepositoryAsync unitRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _unitRepository = unitRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<PagedResponse<IEnumerable<UnitDto>>> Handle(
            GetUnitsQuery request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
            {
                return new PagedResponse<IEnumerable<UnitDto>>(
                    Enumerable.Empty<UnitDto>(),
                    request.Parameter.PageNumber,
                    request.Parameter.PageSize,
                    0);
            }

            var pagedUnits = await _unitRepository.GetUnitsPagedResponseAsync(
                tenantId.Value,
                request.Parameter.Filter,
                request.Parameter.OrderKey,
                request.Parameter.OrderDescending,
                request.Parameter.PageNumber,
                request.Parameter.PageSize,
                cancellationToken);

            var dto = _mapper.Map<IEnumerable<UnitDto>>(pagedUnits.Data);

            return new PagedResponse<IEnumerable<UnitDto>>(
                dto,
                pagedUnits.PageNumber,
                pagedUnits.PageSize,
                pagedUnits.TotalCount);
        }
    }
}
