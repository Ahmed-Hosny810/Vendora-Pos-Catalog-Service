using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.CatalogService.Application.Common.Constants;
using Pos.CatalogService.Application.Features.ProductImages.Commands.CompleteUploadCommand;
using Pos.CatalogService.Application.Features.ProductImages.Commands.RemoveCommand;
using Pos.CatalogService.Application.Features.ProductImages.Commands.StartUploadCommand;
using Pos.CatalogService.Application.Features.ProductImages.DTOS;
using Pos.CatalogService.Application.Features.ProductImages.Queries.GetByProductIdQuery;
using Pos.CatalogService.Application.Wrappers;

namespace Pos.CatalogService.WebApi.Controllers.V1
{
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProductImagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductImagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{productId:guid}")]
        [Authorize(Policy = CatalogPolicies.CanViewCatalog)]
        public async Task<ActionResult<Response<IReadOnlyList<ProductImageDto>>>> GetByProductId(Guid productId,CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetProductImagesQuery
                {
                    ProductId = productId
                },
                cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(
                    new Response<IReadOnlyList<ProductImageDto>>(
                        message: string.Join(", ", result.Errors)));
            }

            return Ok(
                new Response<IReadOnlyList<ProductImageDto>>(
                    data: result.Value));
        }

        [HttpPost("uploads/start")]
        [Authorize(Policy = CatalogPolicies.CanManageCatalog)]
        public async Task<ActionResult<Response<StartProductImageUploadResponse>>>StartUpload([FromBody] StartProductImageUploadCommand command,
                CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                command,
                cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(
                    new Response<StartProductImageUploadResponse>(
                        message: string.Join(", ", result.Errors)));
            }

            return Ok(
                new Response<StartProductImageUploadResponse>(
                    data: result.Value));
        }

        [HttpPost("uploads/complete")]
        [Authorize(Policy = CatalogPolicies.CanManageCatalog)]
        public async Task<ActionResult<Response<Guid>>>CompleteUpload([FromBody] CompleteProductImageUploadCommand command,
                CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                command,
                cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(
                    new Response<Guid>(
                        message: string.Join(", ", result.Errors)));
            }

            return Ok(
                new Response<Guid>(
                    data: result.Value));
        }

        [HttpDelete("{imageId:guid}")]
        [Authorize(policy: CatalogPolicies.CanManageCatalog)]
        public async Task<ActionResult<Response<bool>>> Delete(Guid imageId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new RemoveProductImageCommand
                {
                    ImageId = imageId
                },
                cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(
                    new Response<Guid>(
                        message: string.Join(", ", result.Errors)));
            }

            return Ok(
                new Response<bool>(data: result.IsSuccess,message:"Image deleted successfully"));
        }
    }
}
