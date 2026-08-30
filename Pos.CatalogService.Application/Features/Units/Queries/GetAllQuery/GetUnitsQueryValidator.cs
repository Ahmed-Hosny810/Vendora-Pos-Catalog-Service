using FluentValidation;


namespace Pos.CatalogService.Application.Features.Units.Queries.GetAllQuery
{
    public class GetUnitsQueryValidator : AbstractValidator<GetUnitsQuery>
    {
        public GetUnitsQueryValidator()
        {
            RuleFor(x => x.Parameter.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be greater than or equal to 1.");

            RuleFor(x => x.Parameter.PageSize)
                .InclusiveBetween(1, 50)
                .WithMessage("Page size must be between 1 and 50.");

            RuleFor(x => x.Parameter.Filter!.Name)
                .MaximumLength(80)
                .WithMessage("Unit name filter must not exceed 80 characters.")
                .When(x => x.Parameter.Filter != null &&
                           !string.IsNullOrWhiteSpace(x.Parameter.Filter.Name));

            RuleFor(x => x.Parameter.Filter!.Symbol)
                .MaximumLength(20)
                .WithMessage("Unit symbol filter must not exceed 20 characters.")
                .When(x => x.Parameter.Filter != null &&
                           !string.IsNullOrWhiteSpace(x.Parameter.Filter.Symbol));
        }
    }
}
