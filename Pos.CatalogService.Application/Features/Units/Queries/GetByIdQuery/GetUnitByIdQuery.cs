using AutoMapper;
using MediatR;
using Pos.CatalogService.Application.Features.Units.DTOS;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.Units.Queries.GetByIdQuery
{
    public class GetUnitByIdQuery : IRequest<Result<UnitDto>>
    {
        public Guid UnitId { get; set; }
    }

    public class GetUnitByIdQueryHandler
        : IRequestHandler<GetUnitByIdQuery, Result<UnitDto>>
    {
        private readonly IUnitRepositoryAsync _unitRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetUnitByIdQueryHandler(
            IUnitRepositoryAsync unitRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _unitRepository = unitRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<Result<UnitDto>> Handle(
            GetUnitByIdQuery request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                return Result<UnitDto>.Failure("TenantId claim is missing.");

            var unit = await _unitRepository.GetUnitByIdForTenantAsync(
                tenantId.Value,
                request.UnitId,
                cancellationToken);

            if (unit == null)
                return Result<UnitDto>.Failure($"Unit with Id {request.UnitId} not found.");

            var dto = _mapper.Map<UnitDto>(unit);

            return Result<UnitDto>.Success(dto);
        }
    }
}
