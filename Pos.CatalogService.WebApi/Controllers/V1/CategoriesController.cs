using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.CatalogService.Application.Features.Categories.Commands.ActivateCommand;
using Pos.CatalogService.Application.Features.Categories.Commands.CreateCommand;
using Pos.CatalogService.Application.Features.Categories.Commands.DeactivateCommand;
using Pos.CatalogService.Application.Features.Categories.Commands.UpdateCommand;
using Pos.CatalogService.Application.Features.Categories.DTOS;
using Pos.CatalogService.Application.Features.Categories.Queries.GetAllQuery;
using Pos.CatalogService.Application.Features.Categories.Queries.GetByIdQuery;
using Pos.CatalogService.Application.Wrappers;

namespace Pos.CatalogService.WebApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Response<Guid>>> Create(
            [FromBody] CreateCategoryCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new Response<Guid>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<Guid>(
                data: result.Value,
                message: "Category created successfully."));
        }

        [HttpPut("{categoryId:guid}")]
        public async Task<ActionResult<Response<Guid>>> Update(
            Guid categoryId,
            [FromBody] UpdateCategoryCommand command,
            CancellationToken cancellationToken)
        {
            command.CategoryId = categoryId;

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new Response<Guid>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<Guid>(
                data: result.Value,
                message: "Category updated successfully."));
        }

        [HttpPost("activate")]
        public async Task<ActionResult<Response<bool>>> Activate(
            ActivateCategoryCommand command,
            CancellationToken cancellationToken)
        {

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new Response<bool>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<bool>(
                data: true,
                message: "Category activated successfully."));
        }

        [HttpPost("deactivate")]
        public async Task<ActionResult<Response<bool>>> Deactivate(
            DeactivateCategoryCommand command,
            CancellationToken cancellationToken)
        {

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new Response<bool>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<bool>(
                data: true,
                message: "Category deactivated successfully."));
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResponse<IEnumerable<CategoryDto>>>> GetAll(
            [FromBody] GetAllCategoriesQuery query,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(query, cancellationToken);

            return Ok(response);
        }

        [HttpGet("{categoryId:guid}")]
        public async Task<ActionResult<Response<CategoryDto>>> GetById(
            Guid categoryId,
            CancellationToken cancellationToken)
        {
            var query = new GetCategoryByIdQuery
            {
                CategoryId = categoryId
            };

            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsFailure)
                return NotFound(new Response<CategoryDto>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<CategoryDto>(
                data: result.Value,
                message: "Category retrieved successfully."));
        }
    }
}
