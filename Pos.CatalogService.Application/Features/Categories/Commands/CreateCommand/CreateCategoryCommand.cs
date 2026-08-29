using MediatR;
using Pos.CatalogService.Application.Interfaces;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;
using Pos.CatalogService.Domain.Constants;
using Pos.CatalogService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.Categories.Commands.CreateCommand
{
    public class CreateCategoryCommand : IRequest<Result<Guid>>
    {
        public Guid? ParentCategoryId { get; set; }

        public string? NameAr { get; set; }

        public string NameEn { get; set; } = null!;

        public int SortOrder { get; set; }

        public bool IsVisible { get; set; } = true;
    }

    public class CreateCategoryCommandHandler
        : IRequestHandler<CreateCategoryCommand, Result<Guid>>
    {
        private readonly ICategoryRepositoryAsync _categoryRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCategoryCommandHandler(
            ICategoryRepositoryAsync categoryRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(
            CreateCategoryCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                return Result<Guid>.Failure("TenantId claim is missing.");

            var normalizedNameEn = request.NameEn.Trim();

            var isNameExists = await _categoryRepository.IsCategoryNameExistsAsync(
                tenantId.Value,
                normalizedNameEn,
                cancellationToken);

            if (isNameExists)
                return Result<Guid>.Failure("Category name already exists.");

            if (request.ParentCategoryId.HasValue)
            {
                var isParentValid = await _categoryRepository.IsParentCategoryValidAsync(
                    tenantId.Value,
                    request.ParentCategoryId.Value,
                    cancellationToken);

                if (!isParentValid)
                    return Result<Guid>.Failure("Parent category is invalid.");
            }

            var category = new Category
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId.Value,
                ParentCategoryId = request.ParentCategoryId,
                NameAr = string.IsNullOrWhiteSpace(request.NameAr)
                    ? null
                    : request.NameAr.Trim(),
                NameEn = normalizedNameEn,
                SortOrder = request.SortOrder,
                IsVisible = request.IsVisible,
                Status = CategoryStatuses.Active,
                CreatedAt = DateTime.UtcNow
            };

            await _categoryRepository.AddAsync(category, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(category.Id);
        }
    }
}
