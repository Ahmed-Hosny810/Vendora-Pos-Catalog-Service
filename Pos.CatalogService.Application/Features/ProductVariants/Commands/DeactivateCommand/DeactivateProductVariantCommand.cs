using MediatR;
using Pos.CatalogService.Application.Interfaces;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.ProductVariants.Commands.DeactivateCommand
{
    public class DeactivateProductVariantCommand : IRequest<Result>
    {
        public Guid VariantId { get; set; }
    }

    public class DeactivateProductVariantCommandHandler
        : IRequestHandler<DeactivateProductVariantCommand, Result>
    {
        private readonly IProductVariantRepositoryAsync _variantRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public DeactivateProductVariantCommandHandler(
            IProductVariantRepositoryAsync variantRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _variantRepository = variantRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(
            DeactivateProductVariantCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                return Result.Failure("TenantId claim is missing.");

            var variant = await _variantRepository.GetProductVariantByIdAndTenantIdAsync(
                tenantId.Value,
                request.VariantId,
                cancellationToken);

            if (variant == null)
                return Result.Failure($"Variant with Id {request.VariantId} not found.");

            if (!variant.IsActive)
                return Result.Success();

            variant.Deactivate();

            _variantRepository.Update(variant);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
