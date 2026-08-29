using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.TaxRates.Queries.GetByIdQuery
{
    public class GetTaxRateByIdQueryValidator: AbstractValidator<GetTaxRateByIdQuery>
    {
        public GetTaxRateByIdQueryValidator()
        {
            RuleFor(x => x.TaxRateId)
                .NotEmpty()
                .WithMessage("Tax rate Id is required.");
        }
    }
}
