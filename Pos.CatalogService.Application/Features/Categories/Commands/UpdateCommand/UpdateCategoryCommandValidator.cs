using FluentValidation;
namespace Pos.CatalogService.Application.Features.Categories.Commands.UpdateCommand
{
    public class UpdateCategoryCommandValidator:AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(x => x.CategoryId)
                .NotEmpty()
                .WithMessage("Category Id is required.");

            RuleFor(x => x.NameEn)
                .NotEmpty()
                .WithMessage("English category name is required.")
                .MaximumLength(150)
                .WithMessage("English category name must not exceed 150 characters.");

            RuleFor(x => x.NameAr)
                .MaximumLength(150)
                .WithMessage("Arabic category name must not exceed 150 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.NameAr));

            RuleFor(x => x.SortOrder)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Sort order cannot be negative.");
        }
    }
}
