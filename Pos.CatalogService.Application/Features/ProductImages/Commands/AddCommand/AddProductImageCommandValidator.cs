using FluentValidation;


namespace Pos.CatalogService.Application.Features.ProductImages.Commands.AddCommand
{
    public class AddProductImageCommandValidator
       : AbstractValidator<AddProductImageCommand>
    {
        public AddProductImageCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Product Id is required.");

            RuleFor(x => x.ImageUrl)
                .NotEmpty()
                .WithMessage("Image URL is required.")
                .MaximumLength(500)
                .WithMessage("Image URL must not exceed 500 characters.");

            RuleFor(x => x.SortOrder)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Sort order cannot be negative.");
        }
    }
}
