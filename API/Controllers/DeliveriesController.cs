using API.Contracts.Deliveries;
using API.Extensions;
using Asp.Versioning;
using Application.Features.Deliveries.AssignDeliveryPerson;
using Application.Features.Deliveries.AssignDeliveryPersonToZone;
using Application.Features.Deliveries.CreateDelivery;
using Application.Features.Deliveries.GetAllDeliveries;
using Application.Features.Deliveries.GetDeliveryByOrder;
using Application.Features.Deliveries.GetMyDeliveries;
using Application.Features.Deliveries.RemoveDeliveryPersonFromZone;
using Application.Features.Deliveries.UpdateDeliveryStatus;
using Domain.Constants;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers;

/// <summary>
/// Controlador para gestión de entregas y logística
/// </summary>
[ApiVersion(1)]
[ApiController]
[EnableRateLimiting("GlobalPolicy")]
[Route("api/v{v:apiVersion}/deliveries")]
[Authorize]
public class DeliveriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public DeliveriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene la entrega asociada a un pedido
    /// </summary>
    [HttpGet("order/{orderId:guid}")]
    [ProducesResponseType(typeof(DeliveryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeliveryByOrder(Guid orderId)
    {
        var query = new GetDeliveryByOrderQuery(orderId);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return Ok(result.Value);

        return result.ToProblemDetails();
    }

    /// <summary>
    /// Obtiene las entregas asignadas al repartidor autenticado
    /// </summary>
    [HttpGet("my")]
    [Authorize(Roles = Roles.Empleado)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMyDeliveries(
        [FromQuery] DeliveryStatus? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetMyDeliveriesQuery(status, pageNumber, pageSize);
        var result = await _mediator.Send(query);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails();
    }

    /// <summary>
    /// Obtiene todas las entregas (Administrador y Empleado)
    /// </summary>
    [HttpGet("admin/all")]
    [Authorize(Roles = Roles.AdminOrEmpleado)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllDeliveries(
        [FromQuery] DeliveryStatus? status = null,
        [FromQuery] Guid? deliveryPersonId = null,
        [FromQuery] Guid? zoneId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetAllDeliveriesQuery(status, deliveryPersonId, zoneId, fromDate, toDate, pageNumber, pageSize);
        var result = await _mediator.Send(query);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails();
    }

    /// <summary>
    /// Crea una nueva entrega para un pedido (Administrador y Empleado)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Roles.AdminOrEmpleado)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateDelivery([FromBody] CreateDeliveryRequest request)
    {
        var command = new CreateDeliveryCommand(request.OrderId, request.EstimatedDeliveryTime);
        var result = await _mediator.Send(command);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetDeliveryByOrder),
                new { orderId = request.OrderId },
                new { deliveryId = result.Value })
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Asigna un repartidor a una entrega (Administrador y Empleado)
    /// </summary>
    [HttpPost("{id:guid}/assign")]
    [Authorize(Roles = Roles.AdminOrEmpleado)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignDeliveryPerson(Guid id, [FromBody] AssignDeliveryPersonRequest request)
    {
        var command = new AssignDeliveryPersonCommand(id, request.DeliveryPersonId);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { message = "Repartidor asignado exitosamente" });

        return result.ToProblemDetails();
    }

    /// <summary>
    /// Actualiza el estado de una entrega (Empleado repartidor)
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = Roles.Empleado)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDeliveryStatus(Guid id, [FromBody] UpdateDeliveryStatusRequest request)
    {
        var command = new UpdateDeliveryStatusCommand(
            id,
            request.NewStatus,
            request.Latitude,
            request.Longitude,
            request.Notes);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { message = "Estado de entrega actualizado" });

        return result.ToProblemDetails();
    }

    /// <summary>
    /// Asigna un repartidor a una zona (solo Administrador)
    /// </summary>
    [HttpPost("zones/assign")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AssignDeliveryPersonToZone([FromBody] AssignDeliveryPersonToZoneRequest request)
    {
        var command = new AssignDeliveryPersonToZoneCommand(request.DeliveryPersonId, request.ZoneId);
        var result = await _mediator.Send(command);

        return result.IsSuccess
            ? Ok(new { message = "Repartidor asignado a la zona exitosamente" })
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Elimina la asignación de un repartidor a una zona (solo Administrador)
    /// </summary>
    [HttpDelete("zones/remove")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveDeliveryPersonFromZone([FromBody] AssignDeliveryPersonToZoneRequest request)
    {
        var command = new RemoveDeliveryPersonFromZoneCommand(request.DeliveryPersonId, request.ZoneId);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { message = "Asignación de zona eliminada exitosamente" });

        return result.ToProblemDetails();
    }
}
