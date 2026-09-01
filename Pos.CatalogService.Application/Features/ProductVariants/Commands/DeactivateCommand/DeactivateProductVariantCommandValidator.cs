using FluentValidation;


namespace Pos.CatalogService.Application.Features.ProductVariants.Commands.DeactivateCommand
{
    public class DeactivateProductVariantCommandValidator
        : AbstractValidator<DeactivateProductVariantCommand>
    {
        public DeactivateProductVariantCommandValidator()
        {
            RuleFor(x => x.VariantId)
                .NotEmpty()
                .WithMessage("Variant Id is required.");
        }
    }
}
