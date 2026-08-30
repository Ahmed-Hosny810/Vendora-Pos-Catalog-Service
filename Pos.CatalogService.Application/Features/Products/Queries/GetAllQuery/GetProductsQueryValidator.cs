using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Features.Products.Queries.GetAllQuery
{
    public class GetProductsQueryValidator
       : AbstractValidator<GetProductsQuery>
    {
        public GetProductsQueryValidator()
        {
            RuleFor(x => x.Parameter.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be greater than or equal to 1.");

            RuleFor(x => x.Parameter.PageSize)
                .InclusiveBetween(1, 50)
                .WithMessage("Page size must be between 1 and 50.");

            RuleFor(x => x.Parameter.Filter!.Search)
                .MaximumLength(200)
                .WithMessage("Search must not exceed 200 characters.")
                .When(x => x.Parameter.Filter != null &&
                           !string.IsNullOrWhiteSpace(x.Parameter.Filter.Search));

            RuleFor(x => x.Parameter.Filter!.NameEn)
                .MaximumLength(200)
                .WithMessage("English product name filter must not exceed 200 characters.")
                .When(x => x.Parameter.Filter != null &&
                           !string.IsNullOrWhiteSpace(x.Parameter.Filter.NameEn));

            RuleFor(x => x.Parameter.Filter!.NameAr)
                .MaximumLength(200)
                .WithMessage("Arabic product name filter must not exceed 200 characters.")
                .When(x => x.Parameter.Filter != null &&
                           !string.IsNullOrWhiteSpace(x.Parameter.Filter.NameAr));

            RuleFor(x => x.Parameter.Filter!.Sku)
                .MaximumLength(100)
                .WithMessage("SKU filter must not exceed 100 characters.")
                .When(x => x.Parameter.Filter != null &&
                           !string.IsNullOrWhiteSpace(x.Parameter.Filter.Sku));

            RuleFor(x => x.Parameter.Filter!.Barcode)
                .MaximumLength(100)
                .WithMessage("Barcode filter must not exceed 100 characters.")
                .When(x => x.Parameter.Filter != null &&
                           !string.IsNullOrWhiteSpace(x.Parameter.Filter.Barcode));

            RuleFor(x => x.Parameter.Filter!.Status)
                .MaximumLength(30)
                .WithMessage("Status filter must not exceed 30 characters.")
                .When(x => x.Parameter.Filter != null &&
                           !string.IsNullOrWhiteSpace(x.Parameter.Filter.Status));
        }
    }
}
