using MediatR;
using Pos.CatalogService.Application.Interfaces;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.Categories.Commands.DeactivateCommand
{
    public class DeactivateCategoryCommand : IRequest<Result>
    {
        public Guid CategoryId { get; set; }
    }

    public class DeactivateCategoryCommandHandler
        : IRequestHandler<DeactivateCategoryCommand, Result>
    {
        private readonly ICategoryRepositoryAsync _categoryRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public DeactivateCategoryCommandHandler(
            ICategoryRepositoryAsync categoryRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(
            DeactivateCategoryCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                return Result.Failure("TenantId claim is missing.");

            var category = await _categoryRepository.GetCategoryByIdAndTenantIdAsync(
                tenantId.Value,
                request.CategoryId,
                cancellationToken);

            if (category == null)
                return Result.Failure($"Category with Id {request.CategoryId} not found.");

            if (!category.IsActive)
                return Result.Success();

            category.Deactivate();

            _categoryRepository.Update(category);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
