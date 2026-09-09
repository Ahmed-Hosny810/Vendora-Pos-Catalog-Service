using FluentValidation;


namespace Pos.CatalogService.Application.Features.ProductImages.Commands.CompleteUploadCommand
{
    public class CompleteProductImageUploadCommandValidator
       : AbstractValidator<CompleteProductImageUploadCommand>
    {
        public CompleteProductImageUploadCommandValidator()
        {

            RuleFor(x => x.SortOrder)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Sort order cannot be negative.");
        }
    }
}
