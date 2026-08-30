using FluentValidation;

namespace Pos.CatalogService.Application.Features.Products.Commands.UpdateCommand
{
    public class UpdateProductCommandValidator
       : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Product Id is required.");

            RuleFor(x => x.CategoryId)
                .NotEmpty()
                .WithMessage("Category Id is required.");

            RuleFor(x => x.NameEn)
                .NotEmpty()
                .WithMessage("English product name is required.")
                .MaximumLength(200)
                .WithMessage("English product name must not exceed 200 characters.");

            RuleFor(x => x.NameAr)
                .MaximumLength(200)
                .WithMessage("Arabic product name must not exceed 200 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.NameAr));

            RuleFor(x => x.Sku)
                .MaximumLength(100)
                .WithMessage("SKU must not exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Sku));

            RuleFor(x => x.Barcode)
                .MaximumLength(100)
                .WithMessage("Barcode must not exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Barcode));

            RuleFor(x => x.CostPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Cost price cannot be negative.");

            RuleFor(x => x.SellingPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Selling price cannot be negative.");

            RuleFor(x => x.TaxRateId)
                .NotEmpty()
                .WithMessage("Tax rate Id is required.");

            RuleFor(x => x.UnitId)
                .NotEmpty()
                .WithMessage("Unit Id is required.");
        }
    }
}
