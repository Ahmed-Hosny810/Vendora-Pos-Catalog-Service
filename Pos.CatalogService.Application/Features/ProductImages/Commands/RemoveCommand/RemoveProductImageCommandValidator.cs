using FluentValidation;


namespace Pos.CatalogService.Application.Features.ProductImages.Commands.RemoveCommand
{
    public class RemoveProductImageCommandValidator
        : AbstractValidator<RemoveProductImageCommand>
    {
        public RemoveProductImageCommandValidator()
        {
            RuleFor(x => x.ImageId)
                .NotEmpty()
                .WithMessage("Image Id is required.");
        }
    }
}
