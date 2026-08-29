using MediatR;
using Pos.CatalogService.Application.Interfaces;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.TaxRates.Commands.UpdateCommand
{
    public class UpdateTaxRateCommand : IRequest<Result<Guid>>
    {
        public Guid TaxRateId { get; set; }

        public string Name { get; set; } = null!;

        public decimal Rate { get; set; }

        public bool IsDefault { get; set; }
    }

    public class UpdateTaxRateCommandHandler
        : IRequestHandler<UpdateTaxRateCommand, Result<Guid>>
    {
        private readonly ITaxRateRepositoryAsync _taxRateRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTaxRateCommandHandler(
            ITaxRateRepositoryAsync taxRateRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _taxRateRepository = taxRateRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(
            UpdateTaxRateCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                return Result<Guid>.Failure("TenantId claim is missing.");

            var taxRate = await _taxRateRepository.GetTaxRateByIdAndTenantIdAsync(
                tenantId.Value,
                request.TaxRateId,
                cancellationToken);

            if (taxRate == null)
                return Result<Guid>.Failure($"Tax rate with Id {request.TaxRateId} not found.");

            var normalizedName = request.Name.Trim();

            if (request.IsDefault)
            {
                if (!taxRate.IsActive)
                    return Result<Guid>.Failure("Inactive tax rate cannot be set as default.");

                var currentDefault = await _taxRateRepository.GetDefaultTaxRateAsync(
                    tenantId.Value,
                    cancellationToken);

                if (currentDefault != null &&
                    currentDefault.Id != taxRate.Id)
                {
                    currentDefault.UnmarkAsDefault();
                    _taxRateRepository.Update(currentDefault);
                }
            }

            taxRate.Name = normalizedName;
            taxRate.Rate = request.Rate;
            taxRate.IsDefault = request.IsDefault;
            taxRate.UpdatedAt = DateTime.UtcNow;

            _taxRateRepository.Update(taxRate);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(taxRate.Id);
        }
    }
}
