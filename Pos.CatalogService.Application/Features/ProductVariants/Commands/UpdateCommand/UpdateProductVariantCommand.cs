using MediatR;
using Pos.CatalogService.Application.Interfaces;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Wrappers;

namespace Pos.CatalogService.Application.Features.ProductVariants.Commands.UpdateCommand
{
    public class UpdateProductVariantCommand : IRequest<Result<Guid>>
    {
        public Guid VariantId { get; set; }

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

    public class UpdateProductVariantCommandHandler
        : IRequestHandler<UpdateProductVariantCommand, Result<Guid>>
    {
        private readonly IProductVariantRepositoryAsync _variantRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductVariantCommandHandler(
            IProductVariantRepositoryAsync variantRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _variantRepository = variantRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(
            UpdateProductVariantCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                return Result<Guid>.Failure("TenantId claim is missing.");

            var variant = await _variantRepository.GetProductVariantByIdAndTenantIdAsync(
                tenantId.Value,
                request.VariantId,
                cancellationToken);

            if (variant == null)
                return Result<Guid>.Failure($"Variant with Id {request.VariantId} not found.");

            var normalizedSku = string.IsNullOrWhiteSpace(request.Sku)
                ? null
                : request.Sku.Trim();

            var normalizedBarcode = string.IsNullOrWhiteSpace(request.Barcode)
                ? null
                : request.Barcode.Trim();

            if (!string.IsNullOrWhiteSpace(normalizedSku))
            {
                var skuExistsForAnotherVariant =
                    await _variantRepository.IsSkuExistsForAnotherVariantAsync(
                        tenantId.Value,
                        variant.Id,
                        normalizedSku,
                        cancellationToken);

                if (skuExistsForAnotherVariant)
                    return Result<Guid>.Failure("Variant SKU already exists.");
            }

            if (!string.IsNullOrWhiteSpace(normalizedBarcode))
            {
                var barcodeExistsForAnotherVariant =
                    await _variantRepository.IsBarcodeExistsForAnotherVariantAsync(
                        tenantId.Value,
                        variant.Id,
                        normalizedBarcode,
                        cancellationToken);

                if (barcodeExistsForAnotherVariant)
                    return Result<Guid>.Failure("Variant barcode already exists.");
            }

            variant.Sku = normalizedSku;
            variant.Barcode = normalizedBarcode;
            variant.Name = request.Name.Trim();
            variant.SellingPrice = request.SellingPrice;
            variant.CostPrice = request.CostPrice;
            variant.OptionKey1 = string.IsNullOrWhiteSpace(request.OptionKey1)
                ? null
                : request.OptionKey1.Trim();
            variant.OptionValue1 = string.IsNullOrWhiteSpace(request.OptionValue1)
                ? null
                : request.OptionValue1.Trim();
            variant.OptionKey2 = string.IsNullOrWhiteSpace(request.OptionKey2)
                ? null
                : request.OptionKey2.Trim();
            variant.OptionValue2 = string.IsNullOrWhiteSpace(request.OptionValue2)
                ? null
                : request.OptionValue2.Trim();
            variant.UpdatedAt = DateTime.UtcNow;

            _variantRepository.Update(variant);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(variant.Id);
        }
    }
}
