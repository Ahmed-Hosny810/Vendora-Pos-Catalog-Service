using FluentValidation;


namespace Pos.CatalogService.Application.Features.ProductImages.Commands.SetMainCommand
{
    public class SetMainProductImageCommandValidator
       : AbstractValidator<SetMainProductImageCommand>
    {
        public SetMainProductImageCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Product Id is required.");

            RuleFor(x => x.ImageId)
                .NotEmpty()
                .WithMessage("Image Id is required.");
        }
    }
}
