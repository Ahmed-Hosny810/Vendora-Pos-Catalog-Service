using MediatR;
using Pos.CatalogService.Application.Interfaces;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.Products.Commands.ActivateCommand
{
    public class ActivateProductCommand : IRequest<Result>
    {
        public Guid ProductId { get; set; }
    }

    public class ActivateProductCommandHandler
        : IRequestHandler<ActivateProductCommand, Result>
    {
        private readonly IProductRepositoryAsync _productRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public ActivateProductCommandHandler(
            IProductRepositoryAsync productRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(
            ActivateProductCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                return Result.Failure("TenantId claim is missing.");

            var product = await _productRepository.GetProductByIdAndTenantIdForUpdateAsync(
                      tenantId.Value,
                      request.ProductId,
                      cancellationToken);

            if (product == null)
                return Result.Failure($"Product with Id {request.ProductId} not found.");

            if (product.IsActive)
                return Result.Success();

            product.Activate();

            _productRepository.Update(product);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
