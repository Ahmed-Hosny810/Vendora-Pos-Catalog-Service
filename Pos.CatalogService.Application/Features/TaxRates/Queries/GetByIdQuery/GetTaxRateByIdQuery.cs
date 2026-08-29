using AutoMapper;
using MediatR;
using Pos.CatalogService.Application.Features.TaxRates.DTOS;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.TaxRates.Queries.GetByIdQuery
{
    public class GetTaxRateByIdQuery : IRequest<Result<TaxRateDto>>
    {
        public Guid TaxRateId { get; set; }
    }

    public class GetTaxRateByIdQueryHandler
        : IRequestHandler<GetTaxRateByIdQuery, Result<TaxRateDto>>
    {
        private readonly ITaxRateRepositoryAsync _taxRateRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetTaxRateByIdQueryHandler(
            ITaxRateRepositoryAsync taxRateRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _taxRateRepository = taxRateRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<Result<TaxRateDto>> Handle(
            GetTaxRateByIdQuery request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                return Result<TaxRateDto>.Failure("TenantId claim is missing.");

            var taxRate = await _taxRateRepository.GetTaxRateByIdAndTenantIdAsync(
                tenantId.Value,
                request.TaxRateId,
                cancellationToken);

            if (taxRate == null)
                return Result<TaxRateDto>.Failure($"Tax rate with Id {request.TaxRateId} not found.");

            var dto = _mapper.Map<TaxRateDto>(taxRate);

            return Result<TaxRateDto>.Success(dto);
        }
    }
}
