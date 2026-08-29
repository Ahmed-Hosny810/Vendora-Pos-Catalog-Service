using MediatR;
using Pos.CatalogService.Application.Interfaces;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.TaxRates.Commands.SetDefaultCommand
{
    public class SetDefaultTaxRateCommand : IRequest<Result<Guid>>
    {
        public Guid TaxRateId { get; set; }
    }

    public class SetDefaultTaxRateCommandHandler
        : IRequestHandler<SetDefaultTaxRateCommand, Result<Guid>>
    {
        private readonly ITaxRateRepositoryAsync _taxRateRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public SetDefaultTaxRateCommandHandler(
            ITaxRateRepositoryAsync taxRateRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _taxRateRepository = taxRateRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(
            SetDefaultTaxRateCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                return Result<Guid>.Failure("TenantId claim is missing.");

            var selectedTaxRate = await _taxRateRepository.GetTaxRateByIdAndTenantIdAsync(
                tenantId.Value,
                request.TaxRateId,
                cancellationToken);

            if (selectedTaxRate == null)
                return Result<Guid>.Failure($"Tax rate with Id {request.TaxRateId} not found.");

            if (!selectedTaxRate.IsActive)
                return Result<Guid>.Failure("Inactive tax rate cannot be set as default.");

            if (selectedTaxRate.IsDefault)
                return Result<Guid>.Success(selectedTaxRate.Id);

            var currentDefault = await _taxRateRepository.GetDefaultTaxRateAsync(
                tenantId.Value,
                cancellationToken);

            if (currentDefault != null &&
                currentDefault.Id != selectedTaxRate.Id)
            {
                currentDefault.UnmarkAsDefault();
                _taxRateRepository.Update(currentDefault);
            }

            selectedTaxRate.MarkAsDefault();

            _taxRateRepository.Update(selectedTaxRate);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(selectedTaxRate.Id);
        }
    }
}
