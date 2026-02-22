using API.Contracts.ReturnRequests;
using API.Extensions;
using Application.Features.ReturnRequests.CreateReturnRequest;
using Application.Features.ReturnRequests.GetAllReturnRequests;
using Application.Features.ReturnRequests.GetMyReturnRequests;
using Application.Features.ReturnRequests.ReviewReturnRequest;
using Asp.Versioning;
using Domain.Constants;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers;

/// <summary>
/// Gestión de solicitudes de devolución
/// </summary>
[ApiVersion(1)]
[ApiController]
[EnableRateLimiting("GlobalPolicy")]
[Route("api/v{v:apiVersion}/return-requests")]
[Authorize]
public class ReturnRequestsController : ControllerBase
{
    private readonly ISender _mediator;

    public ReturnRequestsController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene las solicitudes de devolución del usuario autenticado
    /// </summary>
    [HttpGet("mine")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyReturnRequests(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMyReturnRequestsQuery(), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails();
    }

    /// <summary>
    /// Crea una nueva solicitud de devolución
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateReturnRequest(
        [FromBody] CreateReturnRequestRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateReturnRequestCommand(
            request.OrderId, request.Reason, request.EvidenceUrl);

        var result = await _mediator.Send(command, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetMyReturnRequests), new { }, new { id = result.Value })
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Obtiene todas las solicitudes de devolución (solo Admin)
    /// </summary>
    [HttpGet("admin/all")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllReturnRequests(
        [FromQuery] ReturnStatus? status,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllReturnRequestsQuery(status), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails();
    }

    /// <summary>
    /// Revisa (aprueba o rechaza) una solicitud de devolución (solo Admin)
    /// </summary>
    [HttpPatch("{id:guid}/review")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReviewReturnRequest(
        Guid id,
        [FromBody] ReviewReturnRequestRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ReviewReturnRequestCommand(
            id, request.Approve, request.Notes, request.RefundType, request.RefundAmount);

        var result = await _mediator.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblemDetails();
    }
}
