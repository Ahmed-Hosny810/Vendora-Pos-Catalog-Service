using FluentValidation;

namespace Pos.CatalogService.Application.Features.ProductVariants.Queries.GetByProductIdQuery
{
    public class GetProductVariantsQueryValidator
        : AbstractValidator<GetProductVariantsQuery>
    {
        public GetProductVariantsQueryValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Product Id is required.");
        }
    }
}
