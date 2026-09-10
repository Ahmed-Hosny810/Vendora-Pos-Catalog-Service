using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pos.CatalogService.Application.Common.Constants;
using Pos.CatalogService.Application.Features.TaxRates.Commands.ActivateCommand;
using Pos.CatalogService.Application.Features.TaxRates.Commands.CreateCommand;
using Pos.CatalogService.Application.Features.TaxRates.Commands.DeactivateCommand;
using Pos.CatalogService.Application.Features.TaxRates.Commands.SetDefaultCommand;
using Pos.CatalogService.Application.Features.TaxRates.Commands.UpdateCommand;
using Pos.CatalogService.Application.Features.TaxRates.DTOS;
using Pos.CatalogService.Application.Features.TaxRates.Queries.GetAllQuery;
using Pos.CatalogService.Application.Features.TaxRates.Queries.GetByIdQuery;
using Pos.CatalogService.Application.Wrappers;

namespace Pos.CatalogService.WebApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TaxRatesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TaxRatesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Policy = CatalogPolicies.CanManageCatalog)]
        public async Task<ActionResult<Response<Guid>>> Create(
            [FromBody] CreateTaxRateCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new Response<Guid>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<Guid>(
                data: result.Value,
                message: "Tax rate created successfully."));
        }

        [HttpPut]
        [Authorize(Policy = CatalogPolicies.CanManageCatalog)]
        public async Task<ActionResult<Response<Guid>>> Update(
            [FromBody] UpdateTaxRateCommand command,
            CancellationToken cancellationToken)
        {

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new Response<Guid>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<Guid>(
                data: result.Value,
                message: "Tax rate updated successfully."));
        }

        [HttpPost("activate")]
        [Authorize(Policy = CatalogPolicies.CanManageCatalog)]
        public async Task<ActionResult<Response<bool>>> Activate(
            ActivateTaxRateCommand command,
            CancellationToken cancellationToken)
        {

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new Response<bool>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<bool>(
                data: true,
                message: "Tax rate activated successfully."));
        }

        [HttpPost("deactivate")]
        [Authorize(Policy = CatalogPolicies.CanManageCatalog)]
        public async Task<ActionResult<Response<bool>>> Deactivate(
            DeactivateTaxRateCommand command,
            CancellationToken cancellationToken)
        {
  
            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new Response<bool>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<bool>(
                data: true,
                message: "Tax rate deactivated successfully."));
        }

        [HttpPost("set-default")]
        [Authorize(Policy = CatalogPolicies.CanManageCatalog)]
        public async Task<ActionResult<Response<Guid>>> SetDefault(
            SetDefaultTaxRateCommand command,
            CancellationToken cancellationToken)
        {

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(new Response<Guid>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<Guid>(
                data: result.Value,
                message: "Default tax rate updated successfully."));
        }

        [HttpGet]
        [Authorize(Policy = CatalogPolicies.CanViewCatalog)]
        public async Task<ActionResult<Response<IReadOnlyList<TaxRateDto>>>> GetAll(
            CancellationToken cancellationToken)
        {

            var result = await _mediator.Send(new GetTaxRatesQuery(), cancellationToken);

            if (result.IsFailure)
                return BadRequest(new Response<IReadOnlyList<TaxRateDto>>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<IReadOnlyList<TaxRateDto>>(
                data: result.Value,
                message: "Tax rates retrieved successfully."));
        }

        [HttpGet("{taxRateId:guid}")]
        [Authorize(Policy = CatalogPolicies.CanViewCatalog)]
        public async Task<ActionResult<Response<TaxRateDto>>> GetById(
            Guid taxRateId,
            CancellationToken cancellationToken)
        {
            var query = new GetTaxRateByIdQuery
            {
                TaxRateId = taxRateId
            };

            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsFailure)
                return NotFound(new Response<TaxRateDto>(
                    message: string.Join(", ", result.Errors)));

            return Ok(new Response<TaxRateDto>(
                data: result.Value,
                message: "Tax rate retrieved successfully."));
        }
    }
}
