using MediatR;
using Pos.CatalogService.Application.Interfaces;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;

namespace Pos.CatalogService.Application.Features.Products.Commands.UpdateCommand
{
    public class UpdateProductCommand : IRequest<Result<Guid>>
    {
        public Guid ProductId { get; set; }

        public Guid CategoryId { get; set; }

        public string? NameAr { get; set; }

        public string NameEn { get; set; } = null!;

        public string? Sku { get; set; }

        public string? Barcode { get; set; }

        public decimal CostPrice { get; set; }

        public decimal SellingPrice { get; set; }

        public Guid TaxRateId { get; set; }

        public Guid UnitId { get; set; }

        public bool TrackInventory { get; set; }
    }

    public class UpdateProductCommandHandler
        : IRequestHandler<UpdateProductCommand, Result<Guid>>
    {
        private readonly IProductRepositoryAsync _productRepository;
        private readonly ICategoryRepositoryAsync _categoryRepository;
        private readonly IUnitRepositoryAsync _unitRepository;
        private readonly ITaxRateRepositoryAsync _taxRateRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductCommandHandler(
            IProductRepositoryAsync productRepository,
            ICategoryRepositoryAsync categoryRepository,
            IUnitRepositoryAsync unitRepository,
            ITaxRateRepositoryAsync taxRateRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _unitRepository = unitRepository;
            _taxRateRepository = taxRateRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(
            UpdateProductCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                return Result<Guid>.Failure("TenantId claim is missing.");

            var product = await _productRepository.GetProductByIdAndTenantIdForUpdateAsync(
                     tenantId.Value,
                     request.ProductId,
                     cancellationToken);

            if (product == null)
                return Result<Guid>.Failure($"Product with Id {request.ProductId} not found.");

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
                var skuExistsForAnotherProduct =
                    await _productRepository.IsSkuExistsForAnotherProductAsync(
                        tenantId.Value,
                        product.Id,
                        normalizedSku,
                        cancellationToken);

                if (skuExistsForAnotherProduct)
                    return Result<Guid>.Failure("Product SKU already exists.");
            }

            if (!string.IsNullOrWhiteSpace(normalizedBarcode))
            {
                var barcodeExistsForAnotherProduct =
                    await _productRepository.IsBarcodeExistsForAnotherProductAsync(
                        tenantId.Value,
                        product.Id,
                        normalizedBarcode,
                        cancellationToken);

                if (barcodeExistsForAnotherProduct)
                    return Result<Guid>.Failure("Product barcode already exists.");
            }

            product.CategoryId = request.CategoryId;
            product.NameAr = normalizedNameAr;
            product.NameEn = normalizedNameEn;
            product.Sku = normalizedSku;
            product.Barcode = normalizedBarcode;
            product.CostPrice = request.CostPrice;
            product.SellingPrice = request.SellingPrice;
            product.TaxRateId = request.TaxRateId;
            product.UnitId = request.UnitId;
            product.TrackInventory = request.TrackInventory;
            product.UpdatedAt = DateTime.UtcNow;

            _productRepository.Update(product);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(product.Id);
        }
    }
}
