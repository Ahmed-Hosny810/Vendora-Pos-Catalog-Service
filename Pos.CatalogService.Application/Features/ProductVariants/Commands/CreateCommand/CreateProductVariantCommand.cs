using MediatR;
using Pos.CatalogService.Application.Interfaces;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;
using Pos.CatalogService.Domain.Constants;
using Pos.CatalogService.Domain.Models;

namespace Pos.CatalogService.Application.Features.ProductVariants.Commands.CreateCommand
{
    public class CreateProductVariantCommand : IRequest<Result<Guid>>
    {
        public Guid ProductId { get; set; }

        public string? Sku { get; set; }

        public string? Barcode { get; set; }

        public string Name { get; set; } = null!;

        public decimal SellingPrice { get; set; }

        public decimal CostPrice { get; set; }

        public string? OptionKey1 { get; set; }

        public string? OptionValue1 { get; set; }

        public string? OptionKey2 { get; set; }

        public string? OptionValue2 { get; set; }
    }

    public class CreateProductVariantCommandHandler
        : IRequestHandler<CreateProductVariantCommand, Result<Guid>>
    {
        private readonly IProductRepositoryAsync _productRepository;
        private readonly IProductVariantRepositoryAsync _variantRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductVariantCommandHandler(
            IProductRepositoryAsync productRepository,
            IProductVariantRepositoryAsync variantRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _variantRepository = variantRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(
            CreateProductVariantCommand request,
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
                return Result<Guid>.Failure("Product is invalid.");

            var normalizedSku = string.IsNullOrWhiteSpace(request.Sku)
                ? null
                : request.Sku.Trim();

            var normalizedBarcode = string.IsNullOrWhiteSpace(request.Barcode)
                ? null
                : request.Barcode.Trim();

            if (!string.IsNullOrWhiteSpace(normalizedSku))
            {
                var skuExists = await _variantRepository.IsSkuExistsAsync(
                    tenantId.Value,
                    normalizedSku,
                    cancellationToken);

                if (skuExists)
                    return Result<Guid>.Failure("Variant SKU already exists.");
            }

            if (!string.IsNullOrWhiteSpace(normalizedBarcode))
            {
                var barcodeExists = await _variantRepository.IsBarcodeExistsAsync(
                    tenantId.Value,
                    normalizedBarcode,
                    cancellationToken);

                if (barcodeExists)
                    return Result<Guid>.Failure("Variant barcode already exists.");
            }

            var variant = new ProductVariant
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId.Value,
                ProductId = product.Id,
                Sku = normalizedSku,
                Barcode = normalizedBarcode,
                Name = request.Name.Trim(),
                SellingPrice = request.SellingPrice,
                CostPrice = request.CostPrice,
                OptionKey1 = string.IsNullOrWhiteSpace(request.OptionKey1)
                    ? null: request.OptionKey1.Trim(),
                OptionValue1 = string.IsNullOrWhiteSpace(request.OptionValue1)
                    ? null: request.OptionValue1.Trim(),
                OptionKey2 = string.IsNullOrWhiteSpace(request.OptionKey2)
                    ? null: request.OptionKey2.Trim(),
                OptionValue2 = string.IsNullOrWhiteSpace(request.OptionValue2)
                    ? null: request.OptionValue2.Trim(),
                Status = ProductVariantStatuses.Active,
                CreatedAt = DateTime.UtcNow
            };

            await _variantRepository.AddAsync(variant, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(variant.Id);
        }
    }
}
