using FluentValidation;


namespace Pos.CatalogService.Application.Features.ProductImages.Queries.GetByProductIdQuery
{
    public class GetProductImagesQueryValidator
        : AbstractValidator<GetProductImagesQuery>
    {
        public GetProductImagesQueryValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Product Id is required.");
        }
    }
}
