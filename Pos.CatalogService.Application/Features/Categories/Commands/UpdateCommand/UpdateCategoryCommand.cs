using MediatR;
using Pos.CatalogService.Application.Interfaces;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.Categories.Commands.UpdateCommand
{
    public class UpdateCategoryCommand : IRequest<Result<Guid>>
    {
        public Guid CategoryId { get; set; }

        public Guid? ParentCategoryId { get; set; }

        public string? NameAr { get; set; }

        public string NameEn { get; set; } = null!;

        public int SortOrder { get; set; }

        public bool IsVisible { get; set; }
    }

    public class UpdateCategoryCommandHandler
        : IRequestHandler<UpdateCategoryCommand, Result<Guid>>
    {
        private readonly ICategoryRepositoryAsync _categoryRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCategoryCommandHandler(
            ICategoryRepositoryAsync categoryRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(
            UpdateCategoryCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                return Result<Guid>.Failure("TenantId claim is missing.");

            var category = await _categoryRepository.GetCategoryByIdAndTenantIdAsync(
                tenantId.Value,
                request.CategoryId,
                cancellationToken);

            if (category == null)
                return Result<Guid>.Failure($"Category with Id {request.CategoryId} not found.");

            if (request.ParentCategoryId.HasValue &&
                request.ParentCategoryId.Value == category.Id)
            {
                return Result<Guid>.Failure("Category cannot be its own parent.");
            }

            if (request.ParentCategoryId.HasValue)
            {
                var isParentValid = await _categoryRepository.IsParentCategoryValidAsync(
                    tenantId.Value,
                    request.ParentCategoryId.Value,
                    cancellationToken);

                if (!isParentValid)
                    return Result<Guid>.Failure("Parent category is invalid.");
            }

            var normalizedNameEn = request.NameEn.Trim();

            var isNameExistsForAnotherCategory =
                await _categoryRepository.IsCategoryNameExistsForAnotherCategoryAsync(
                    tenantId.Value,
                    category.Id,
                    normalizedNameEn,
                    cancellationToken);

            if (isNameExistsForAnotherCategory)
                return Result<Guid>.Failure("Category name already exists.");

            category.ParentCategoryId = request.ParentCategoryId;
            category.NameAr = string.IsNullOrWhiteSpace(request.NameAr)
                ? null
                : request.NameAr.Trim();
            category.NameEn = normalizedNameEn;
            category.SortOrder = request.SortOrder;
            category.IsVisible = request.IsVisible;
            category.UpdatedAt = DateTime.UtcNow;

            _categoryRepository.Update(category);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(category.Id);
        }
    }
}
