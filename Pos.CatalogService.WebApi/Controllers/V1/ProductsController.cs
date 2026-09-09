using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.CatalogService.Application.Features.Products.Commands.ActivateCommand;
using Pos.CatalogService.Application.Features.Products.Commands.CreateCommand;
using Pos.CatalogService.Application.Features.Products.Commands.DeactivateCommand;
using Pos.CatalogService.Application.Features.Products.Commands.UpdateCommand;
using Pos.CatalogService.Application.Features.Products.DTOS;
using Pos.CatalogService.Application.Features.Products.Queries.GetAllQuery;
using Pos.CatalogService.Application.Features.Products.Queries.GetByIdQuery;
using Pos.CatalogService.Application.Wrappers;

namespace Pos.CatalogService.WebApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Response<Guid>>> Create(
            [FromBody] CreateProductCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new Response<Guid>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<Guid>(
                data: result.Value,
                message: "Product created successfully."));
        }

        [HttpPut]
        public async Task<ActionResult<Response<Guid>>> Update(
            [FromBody] UpdateProductCommand command,
            CancellationToken cancellationToken)
        {

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new Response<Guid>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<Guid>(
                data: result.Value,
                message: "Product updated successfully."));
        }

        [HttpPost("activate")]
        public async Task<ActionResult<Response<bool>>> Activate(
            ActivateProductCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new Response<bool>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<bool>(
                data: true,
                message: "Product activated successfully."));
        }

        [HttpPost("deactivate")]
        public async Task<ActionResult<Response<bool>>> Deactivate(
            DeactivateProductCommand command,
            CancellationToken cancellationToken)
        {

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new Response<bool>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<bool>(
                data: true,
                message: "Product deactivated successfully."));
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResponse<IEnumerable<ProductDto>>>> GetAll(
            [FromBody] GetProductsQuery query,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(query, cancellationToken);

            return Ok(response);
        }

        [HttpPost("details")]
        public async Task<ActionResult<Response<ProductDto>>> GetDetailsById(
            GetProductByIdQuery query,
            CancellationToken cancellationToken)
        {

            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsFailure)
                return NotFound(new Response<ProductDto>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<ProductDto>(
                data: result.Value,
                message: "Product retrieved successfully."));
        }

        [HttpGet("{productId:guid}")]
        public async Task<ActionResult<Response<ProductDto>>> GetById(Guid productId,CancellationToken cancellationToken)
        {
            var query = new GetProductByIdQuery
            {
                ProductId = productId,
                Includes = null
            };

            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsFailure)
                return NotFound(new Response<ProductDto>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<ProductDto>(
                data: result.Value,
                message: "Product retrieved successfully."));
        }
    }
}
