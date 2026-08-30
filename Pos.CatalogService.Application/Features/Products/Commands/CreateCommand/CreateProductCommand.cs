using MediatR;
using Pos.CatalogService.Application.Features.Products.DTOS;
using Pos.CatalogService.Application.Interfaces;
using Pos.CatalogService.Application.Interfaces.Clients;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;
using Pos.CatalogService.Domain.Constants;
using Pos.CatalogService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.Products.Commands.CreateCommand
{
    public class CreateProductCommand : IRequest<Result<Guid>>
    {
        public Guid CategoryId { get; set; }

        public string? NameAr { get; set; }

        public string NameEn { get; set; } = null!;

        public string? Sku { get; set; }

        public string? Barcode { get; set; }

        public decimal CostPrice { get; set; }

        public decimal SellingPrice { get; set; }

        public Guid TaxRateId { get; set; }

        public Guid UnitId { get; set; }

        public bool TrackInventory { get; set; } = true;
    }

    public class CreateProductCommandHandler
        : IRequestHandler<CreateProductCommand, Result<Guid>>
    {
        private readonly IProductRepositoryAsync _productRepository;
        private readonly ICategoryRepositoryAsync _categoryRepository;
        private readonly IUnitRepositoryAsync _unitRepository;
        private readonly ITaxRateRepositoryAsync _taxRateRepository;
        private readonly ITenantBillingClient _tenantBillingClient;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductCommandHandler(IProductRepositoryAsync productRepository,ICategoryRepositoryAsync categoryRepository,
            IUnitRepositoryAsync unitRepository,ITaxRateRepositoryAsync taxRateRepository,ITenantBillingClient tenantBillingClient,
            ICurrentUserService currentUserService,IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _unitRepository = unitRepository;
            _taxRateRepository = taxRateRepository;
            _tenantBillingClient = tenantBillingClient;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(
            CreateProductCommand request,
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
                return Result<Guid>.Failure("Category is invalid.");

            if (!category.IsActive)
                return Result<Guid>.Failure("Category is inactive.");

            var unit = await _unitRepository.GetUnitByIdForTenantAsync(
                tenantId.Value,
                request.UnitId,
                cancellationToken);

            if (unit == null)
                return Result<Guid>.Failure("Unit is invalid.");

            var taxRate = await _taxRateRepository.GetTaxRateByIdAndTenantIdAsync(
                tenantId.Value,
                request.TaxRateId,
                cancellationToken);

            if (taxRate == null)
                return Result<Guid>.Failure("Tax rate is invalid.");

            if (!taxRate.IsActive)
                return Result<Guid>.Failure("Tax rate is inactive.");

            var normalizedNameEn = request.NameEn.Trim();

            var normalizedNameAr = string.IsNullOrWhiteSpace(request.NameAr)
                ? null
                : request.NameAr.Trim();

            var normalizedSku = string.IsNullOrWhiteSpace(request.Sku)
                ? null
                : request.Sku.Trim();

            var normalizedBarcode = string.IsNullOrWhiteSpace(request.Barcode)
                ? null
                : request.Barcode.Trim();

            if (!string.IsNullOrWhiteSpace(normalizedSku))
            {
                var skuExists = await _productRepository.IsSkuExistsAsync(
                    tenantId.Value,
                    normalizedSku,
                    cancellationToken);

                if (skuExists)
                    return Result<Guid>.Failure("Product SKU already exists.");
            }

            if (!string.IsNullOrWhiteSpace(normalizedBarcode))
            {
                var barcodeExists = await _productRepository.IsBarcodeExistsAsync(
                    tenantId.Value,
                    normalizedBarcode,
                    cancellationToken);

                if (barcodeExists)
                    return Result<Guid>.Failure("Product barcode already exists.");
            }

            var product = new Product
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId.Value,
                CategoryId = request.CategoryId,
                NameAr = normalizedNameAr,
                NameEn = normalizedNameEn,
                Sku = normalizedSku,
                Barcode = normalizedBarcode,
                CostPrice = request.CostPrice,
                SellingPrice = request.SellingPrice,
                TaxRateId = request.TaxRateId,
                UnitId = request.UnitId,
                TrackInventory = request.TrackInventory,
                Status = ProductStatuses.Active,
                CreatedAt = DateTime.UtcNow
            };

            await _productRepository.AddAsync(product, cancellationToken);

            var increaseUsageResult = await _tenantBillingClient.IncreaseProductUsageAsync(new ProductUsageRequest 
            { 
                TenantId=tenantId.Value
            },
                cancellationToken);

            if (increaseUsageResult.IsFailure)
                return Result<Guid>.Failure(increaseUsageResult.Errors.ToArray());

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(product.Id);
        }
    }
}
