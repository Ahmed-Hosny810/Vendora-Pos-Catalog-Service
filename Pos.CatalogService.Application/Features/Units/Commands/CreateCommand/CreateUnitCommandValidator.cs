using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.Units.Commands.CreateCommand
{
    public class CreateUnitCommandValidator: AbstractValidator<CreateUnitCommand>
    {
        public CreateUnitCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Unit name is required.")
                .MaximumLength(80)
                .WithMessage("Unit name must not exceed 80 characters.");

            RuleFor(x => x.Symbol)
                .NotEmpty()
                .WithMessage("Unit symbol is required.")
                .MaximumLength(20)
                .WithMessage("Unit symbol must not exceed 20 characters.");
        }
    }
}
