using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pos.CatalogService.Application.Common.Constants;
using Pos.CatalogService.Application.Features.Units.Commands.CreateCommand;
using Pos.CatalogService.Application.Features.Units.Commands.UpdateCommand;
using Pos.CatalogService.Application.Features.Units.DTOS;
using Pos.CatalogService.Application.Features.Units.Queries.GetAllQuery;
using Pos.CatalogService.Application.Features.Units.Queries.GetByIdQuery;
using Pos.CatalogService.Application.Wrappers;

namespace Pos.CatalogService.WebApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class UnitsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UnitsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Policy = CatalogPolicies.CanManageCatalog)]
        public async Task<ActionResult<Response<Guid>>> Create(
            [FromBody] CreateUnitCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new Response<Guid>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<Guid>(
                data: result.Value,
                message: "Unit created successfully."));
        }

        [HttpPut]
        [Authorize(Policy = CatalogPolicies.CanManageCatalog)]
        public async Task<ActionResult<Response<Guid>>> Update(
            [FromBody] UpdateUnitCommand command,
            CancellationToken cancellationToken)
        {

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new Response<Guid>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<Guid>(
                data: result.Value,
                message: "Unit updated successfully."));
        }

        [HttpPost("search")]
        [Authorize(Policy = CatalogPolicies.CanViewCatalog)]
        public async Task<ActionResult<PagedResponse<IEnumerable<UnitDto>>>> GetAll(
            [FromBody] GetUnitsQuery query,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(query, cancellationToken);

            return Ok(response);
        }

        [HttpGet("{unitId:guid}")]
        [Authorize(Policy = CatalogPolicies.CanViewCatalog)]
        public async Task<ActionResult<Response<UnitDto>>> GetById(
            Guid unitId,
            CancellationToken cancellationToken)
        {
            var query = new GetUnitByIdQuery
            {
                UnitId = unitId
            };

            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsFailure)
                return NotFound(new Response<UnitDto>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<UnitDto>(
                data: result.Value,
                message: "Unit retrieved successfully."));
        }
    }
}
