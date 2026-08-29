using MediatR;
using Pos.CatalogService.Application.Interfaces;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.TaxRates.Commands.ActivateCommand
{
    public class ActivateTaxRateCommand : IRequest<Result>
    {
        public Guid TaxRateId { get; set; }
    }

    public class ActivateTaxRateCommandHandler: IRequestHandler<ActivateTaxRateCommand, Result>
    {
        private readonly ITaxRateRepositoryAsync _taxRateRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public ActivateTaxRateCommandHandler(
            ITaxRateRepositoryAsync taxRateRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _taxRateRepository = taxRateRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(
            ActivateTaxRateCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                return Result.Failure("TenantId claim is missing.");

            var taxRate = await _taxRateRepository.GetTaxRateByIdAndTenantIdAsync(
                tenantId.Value,
                request.TaxRateId,
                cancellationToken);

            if (taxRate == null)
                return Result.Failure($"Tax rate with Id {request.TaxRateId} not found.");

            if (taxRate.IsActive)
                return Result.Success();

            taxRate.Activate();

            _taxRateRepository.Update(taxRate);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
