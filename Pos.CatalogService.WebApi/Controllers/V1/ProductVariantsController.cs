using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.CatalogService.Application.Features.ProductVariants.Commands.ActivateCommand;
using Pos.CatalogService.Application.Features.ProductVariants.Commands.CreateCommand;
using Pos.CatalogService.Application.Features.ProductVariants.Commands.DeactivateCommand;
using Pos.CatalogService.Application.Features.ProductVariants.Commands.UpdateCommand;
using Pos.CatalogService.Application.Features.ProductVariants.DTOS;
using Pos.CatalogService.Application.Features.ProductVariants.Queries.GetByProductIdQuery;
using Pos.CatalogService.Application.Wrappers;

namespace Pos.CatalogService.WebApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductVariantsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductVariantsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Response<Guid>>> Create(
            [FromBody] CreateProductVariantCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new Response<Guid>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<Guid>(
                data: result.Value,
                message: "Product variant created successfully."));
        }

        [HttpPut]
        public async Task<ActionResult<Response<Guid>>> Update(
            [FromBody] UpdateProductVariantCommand command,
            CancellationToken cancellationToken)
        {

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new Response<Guid>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<Guid>(
                data: result.Value,
                message: "Product variant updated successfully."));
        }

        [HttpPost("activate")]
        public async Task<ActionResult<Response<bool>>> Activate(
            ActivateProductVariantCommand command,
            CancellationToken cancellationToken)
        {

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new Response<bool>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<bool>(
                data: true,
                message: "Product variant activated successfully."));
        }

        [HttpPost("{variantId:guid}/deactivate")]
        public async Task<ActionResult<Response<bool>>> Deactivate(
             ActivateProductVariantCommand command,
            CancellationToken cancellationToken)
        {

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new Response<bool>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<bool>(
                data: true,
                message: "Product variant deactivated successfully."));
        }

        [HttpGet("{productId:guid}")]
        public async Task<ActionResult<Response<IReadOnlyList<ProductVariantDto>>>> GetByProductId(
            Guid productId,
            CancellationToken cancellationToken)
        {
            var query = new GetProductVariantsQuery
            {
                ProductId = productId
            };

            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new Response<IReadOnlyList<ProductVariantDto>>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<IReadOnlyList<ProductVariantDto>>(
                data: result.Value,
                message: "Product variants retrieved successfully."));
        }
    }
}
