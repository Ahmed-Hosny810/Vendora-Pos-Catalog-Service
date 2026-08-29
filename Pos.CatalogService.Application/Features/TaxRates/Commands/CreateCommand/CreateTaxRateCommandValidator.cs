using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.TaxRates.Commands.CreateCommand
{
    public class CreateTaxRateCommandValidator
        : AbstractValidator<CreateTaxRateCommand>
    {
        public CreateTaxRateCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Tax rate name is required.")
                .MaximumLength(80)
                .WithMessage("Tax rate name must not exceed 80 characters.");

            RuleFor(x => x.Rate)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Tax rate cannot be negative.")
                .LessThanOrEqualTo(100)
                .WithMessage("Tax rate cannot exceed 100.");
        }
    }
}
