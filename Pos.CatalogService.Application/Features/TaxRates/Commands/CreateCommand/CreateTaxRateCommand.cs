using MediatR;
using Pos.CatalogService.Application.Interfaces;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;
using Pos.CatalogService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.TaxRates.Commands.CreateCommand
{
    public class CreateTaxRateCommand : IRequest<Result<Guid>>
    {
        public string Name { get; set; } = null!;

        public decimal Rate { get; set; }

        public bool IsDefault { get; set; }
    }

    public class CreateTaxRateCommandHandler
        : IRequestHandler<CreateTaxRateCommand, Result<Guid>>
    {
        private readonly ITaxRateRepositoryAsync _taxRateRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateTaxRateCommandHandler(
            ITaxRateRepositoryAsync taxRateRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _taxRateRepository = taxRateRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(
            CreateTaxRateCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                return Result<Guid>.Failure("TenantId claim is missing.");

            var normalizedName = request.Name.Trim();

            var isNameExists = await _taxRateRepository.IsTaxRateNameExistsAsync(
                tenantId.Value,
                normalizedName,
                cancellationToken);

            if (isNameExists)
                return Result<Guid>.Failure("Tax rate name already exists.");

            if (request.IsDefault)
            {
                var currentDefault = await _taxRateRepository.GetDefaultTaxRateAsync(
                    tenantId.Value,
                    cancellationToken);

                if (currentDefault != null)
                {
                    currentDefault.UnmarkAsDefault();
                    _taxRateRepository.Update(currentDefault);
                }
            }

            var taxRate = new TaxRate
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId.Value,
                Name = normalizedName,
                Rate = request.Rate,
                IsDefault = request.IsDefault,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _taxRateRepository.AddAsync(taxRate, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(taxRate.Id);
        }
    }
}
