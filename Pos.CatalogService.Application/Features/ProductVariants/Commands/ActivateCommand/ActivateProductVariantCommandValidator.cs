
using FluentValidation;

namespace Pos.CatalogService.Application.Features.ProductVariants.Commands.ActivateCommand
{
    public class ActivateProductVariantCommandValidator : AbstractValidator<ActivateProductVariantCommand>
    {
        public ActivateProductVariantCommandValidator()
        {
            RuleFor(x => x.VariantId)
                .NotEmpty()
                .WithMessage("Variant Id is required.");
        }
    }
}
