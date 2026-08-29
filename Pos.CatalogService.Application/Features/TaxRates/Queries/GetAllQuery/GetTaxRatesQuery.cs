using AutoMapper;
using MediatR;
using Pos.CatalogService.Application.Features.TaxRates.DTOS;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.TaxRates.Queries.GetAllQuery
{
    public class GetTaxRatesQuery : IRequest<Result<IReadOnlyList<TaxRateDto>>>
    {
    }

    public class GetTaxRatesQueryHandler: IRequestHandler<GetTaxRatesQuery, Result<IReadOnlyList<TaxRateDto>>>
    {
        private readonly ITaxRateRepositoryAsync _taxRateRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetTaxRatesQueryHandler(
            ITaxRateRepositoryAsync taxRateRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _taxRateRepository = taxRateRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<Result<IReadOnlyList<TaxRateDto>>> Handle(
            GetTaxRatesQuery request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                return Result<IReadOnlyList<TaxRateDto>>.Failure("TenantId claim is missing.");

            var taxRates = await _taxRateRepository.GetTaxRatesByTenantIdAsync(
                tenantId.Value,
                cancellationToken);

            var dto = _mapper.Map<IReadOnlyList<TaxRateDto>>(taxRates);

            return Result<IReadOnlyList<TaxRateDto>>.Success(dto);
        }
    }
}
