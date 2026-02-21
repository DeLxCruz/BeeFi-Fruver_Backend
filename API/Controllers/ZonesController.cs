using API.Contracts.Common;
using API.Contracts.Zones;
using Application.Features.Zones.AssignFruverToZone;
using Application.Features.Zones.CreateZone;
using Application.Features.Zones.GetZoneById;
using Application.Features.Zones.GetZoneFruvers;
using Application.Features.Zones.GetZones;
using Application.Features.Zones.RemoveFruverFromZone;
using Application.Features.Zones.UpdateZone;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Gestión de zonas de entrega
/// </summary>
[ApiController]
[Route("api/v1/zones")]
public class ZonesController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly ILogger<ZonesController> _logger;

    public ZonesController(ISender mediator, ILogger<ZonesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las zonas
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<ZoneDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetZones(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetZonesQuery(), cancellationToken);
        return Ok(result.Value);
    }

    /// <summary>
    /// Obtiene una zona por su Id
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ZoneDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetZoneById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetZoneByIdQuery(id), cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(new ErrorResponse(
                code: result.Error.Code,
                message: result.Error.Message,
                traceId: HttpContext.TraceIdentifier,
                path: HttpContext.Request.Path));
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Obtiene los fruvers asignados a una zona
    /// </summary>
    [HttpGet("{id:guid}/fruvers")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<ZoneFruverDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetZoneFruvers(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetZoneFruversQuery(id), cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(new ErrorResponse(
                code: result.Error.Code,
                message: result.Error.Message,
                traceId: HttpContext.TraceIdentifier,
                path: HttpContext.Request.Path));
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Crea una nueva zona
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(CreateZoneResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateZone(
        [FromBody] CreateZoneRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateZoneCommand(
            request.Name,
            request.City,
            request.Department,
            request.DeliveryBaseCost);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            var errorResponse = new ErrorResponse(
                code: result.Error.Code,
                message: result.Error.Message,
                traceId: HttpContext.TraceIdentifier,
                path: HttpContext.Request.Path);

            return result.Error.Code == "Zone.AlreadyExists"
                ? Conflict(errorResponse)
                : BadRequest(errorResponse);
        }

        return CreatedAtAction(
            nameof(GetZoneById),
            new { id = result.Value.Id },
            result.Value);
    }

    /// <summary>
    /// Actualiza una zona existente
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateZone(
        Guid id,
        [FromBody] UpdateZoneRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateZoneCommand(
            id,
            request.Name,
            request.City,
            request.Department,
            request.DeliveryBaseCost,
            request.IsActive);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            var errorResponse = new ErrorResponse(
                code: result.Error.Code,
                message: result.Error.Message,
                traceId: HttpContext.TraceIdentifier,
                path: HttpContext.Request.Path);

            return result.Error.Code == "Zone.NotFound"
                ? NotFound(errorResponse)
                : BadRequest(errorResponse);
        }

        return NoContent();
    }

    /// <summary>
    /// Asigna un fruver a una zona
    /// </summary>
    [HttpPost("{id:guid}/fruvers")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AssignFruver(
        Guid id,
        [FromBody] AssignFruverRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AssignFruverToZoneCommand(id, request.FruverId);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            var errorResponse = new ErrorResponse(
                code: result.Error.Code,
                message: result.Error.Message,
                traceId: HttpContext.TraceIdentifier,
                path: HttpContext.Request.Path);

            return result.Error.Code switch
            {
                "Zone.NotFound" or "Zone.FruverNotFound" => NotFound(errorResponse),
                "Zone.FruverAlreadyAssigned" => Conflict(errorResponse),
                _ => BadRequest(errorResponse)
            };
        }

        return Ok(new { message = "Fruver asignado a la zona exitosamente" });
    }

    /// <summary>
    /// Elimina un fruver de una zona
    /// </summary>
    [HttpDelete("{id:guid}/fruvers/{fruverId:guid}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFruver(
        Guid id,
        Guid fruverId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveFruverFromZoneCommand(id, fruverId);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(new ErrorResponse(
                code: result.Error.Code,
                message: result.Error.Message,
                traceId: HttpContext.TraceIdentifier,
                path: HttpContext.Request.Path));
        }

        return NoContent();
    }
}
