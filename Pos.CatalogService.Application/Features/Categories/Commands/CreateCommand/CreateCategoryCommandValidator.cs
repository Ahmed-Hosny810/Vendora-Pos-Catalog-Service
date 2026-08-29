using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.Categories.Commands.CreateCommand
{
    public class CreateCategoryCommandValidator:AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
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
