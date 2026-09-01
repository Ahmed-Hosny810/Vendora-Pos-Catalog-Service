using FluentValidation;

namespace Pos.CatalogService.Application.Features.ProductVariants.Commands.UpdateCommand
{
    public class UpdateProductVariantCommandValidator
        : AbstractValidator<UpdateProductVariantCommand>
    {
        public UpdateProductVariantCommandValidator()
        {
            RuleFor(x => x.VariantId)
                .NotEmpty()
                .WithMessage("Variant Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Variant name is required.")
                .MaximumLength(200)
                .WithMessage("Variant name must not exceed 200 characters.");

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

            RuleFor(x => x.OptionKey1)
                .MaximumLength(50)
                .WithMessage("Option key 1 must not exceed 50 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.OptionKey1));

            RuleFor(x => x.OptionValue1)
                .MaximumLength(50)
                .WithMessage("Option value 1 must not exceed 50 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.OptionValue1));

            RuleFor(x => x.OptionKey2)
                .MaximumLength(50)
                .WithMessage("Option key 2 must not exceed 50 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.OptionKey2));

            RuleFor(x => x.OptionValue2)
                .MaximumLength(50)
                .WithMessage("Option value 2 must not exceed 50 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.OptionValue2));
        }
    }
}
