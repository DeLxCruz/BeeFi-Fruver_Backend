using API.Contracts.Notifications;
using API.Extensions;
using Asp.Versioning;
using Application.Features.Notifications.GetMyNotifications;
using Application.Features.Notifications.MarkAllAsRead;
using Application.Features.Notifications.MarkAsRead;
using Application.Features.Notifications.RegisterDeviceToken;
using Application.Features.Notifications.SendNotification;
using Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers;

/// <summary>
/// Controlador para notificaciones y dispositivos push
/// </summary>
[ApiVersion(1)]
[ApiController]
[EnableRateLimiting("GlobalPolicy")]
[Route("api/v{v:apiVersion}/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene las notificaciones del usuario autenticado
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyNotifications(
        [FromQuery] bool? isRead = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetMyNotificationsQuery(isRead, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails();
    }

    /// <summary>
    /// Marca una notificación como leída
    /// </summary>
    [HttpPatch("{id:guid}/read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var command = new MarkNotificationAsReadCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { message = "Notificación marcada como leída" });

        return result.ToProblemDetails();
    }

    /// <summary>
    /// Marca todas las notificaciones como leídas
    /// </summary>
    [HttpPatch("read-all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var command = new MarkAllNotificationsAsReadCommand();
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? Ok(new { message = "Todas las notificaciones marcadas como leídas" })
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Registra un token de dispositivo para notificaciones push
    /// </summary>
    [HttpPost("device-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RegisterDeviceToken([FromBody] RegisterDeviceTokenRequest request)
    {
        var command = new RegisterDeviceTokenCommand(request.Token, request.Platform);
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? Ok(new { message = "Token de dispositivo registrado exitosamente" })
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Envía una notificación a un usuario específico (solo Administrador)
    /// </summary>
    [HttpPost("send")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SendNotification([FromBody] SendNotificationRequest request)
    {
        var command = new SendNotificationCommand(
            request.UserId, request.Title, request.Message, request.Type, request.Data);
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? Ok(new { message = "Notificación enviada exitosamente" })
            : result.ToProblemDetails();
    }
}
