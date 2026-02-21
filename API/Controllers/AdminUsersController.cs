using Application.Features.Users.ApproveUser;
using Application.Features.Users.GetPendingUsers;
using Application.Features.Users.RejectUser;
using Application.Features.Users.SuspendUser;
using Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controlador para administración de usuarios
/// </summary>
[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = Roles.Administrador)]
public class AdminUsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminUsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene la lista de usuarios pendientes de aprobación
    /// </summary>
    /// <returns>Lista de usuarios pendientes</returns>
    [HttpGet("pending")]
    [ProducesResponseType(typeof(List<PendingUserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingUsers()
    {
        var query = new GetPendingUsersQuery();
        var result = await _mediator.Send(query);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    /// <summary>
    /// Aprueba una cuenta de usuario pendiente
    /// </summary>
    /// <param name="userId">ID del usuario a aprobar</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPost("{userId:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveUser(Guid userId)
    {
        var command = new ApproveUserCommand(userId);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(new { message = "Usuario aprobado exitosamente" });
        }

        return result.Error.Code == "User.NotFound"
            ? NotFound(result.Error)
            : BadRequest(result.Error);
    }

    /// <summary>
    /// Rechaza una solicitud de cuenta
    /// </summary>
    /// <param name="userId">ID del usuario a rechazar</param>
    /// <param name="request">Razón del rechazo</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPost("{userId:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectUser(Guid userId, [FromBody] RejectUserRequest request)
    {
        var command = new RejectUserCommand(userId, request.Reason);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(new { message = "Usuario rechazado exitosamente" });
        }

        return result.Error.Code == "User.NotFound"
            ? NotFound(result.Error)
            : BadRequest(result.Error);
    }

    /// <summary>
    /// Suspende una cuenta de usuario
    /// </summary>
    /// <param name="userId">ID del usuario a suspender</param>
    /// <param name="request">Razón de la suspensión</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPost("{userId:guid}/suspend")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SuspendUser(Guid userId, [FromBody] SuspendUserRequest request)
    {
        var command = new SuspendUserCommand(userId, request.Reason);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(new { message = "Usuario suspendido exitosamente" });
        }

        return result.Error.Code == "User.NotFound"
            ? NotFound(result.Error)
            : BadRequest(result.Error);
    }
}

/// <summary>
/// Request para rechazar usuario
/// </summary>
public record RejectUserRequest(string Reason);

/// <summary>
/// Request para suspender usuario
/// </summary>
public record SuspendUserRequest(string Reason);
