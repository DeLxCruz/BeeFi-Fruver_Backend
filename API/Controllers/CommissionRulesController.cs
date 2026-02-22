using API.Contracts.CommissionRules;
using API.Extensions;
using Application.Features.CommissionRules.CreateCommissionRule;
using Application.Features.CommissionRules.DeleteCommissionRule;
using Application.Features.CommissionRules.GetCommissionRules;
using Application.Features.CommissionRules.SimulateCommission;
using Application.Features.CommissionRules.UpdateCommissionRule;
using Asp.Versioning;
using Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers;

/// <summary>
/// Gestión de reglas de comisión
/// </summary>
[ApiVersion(1)]
[ApiController]
[EnableRateLimiting("GlobalPolicy")]
[Route("api/v{v:apiVersion}/commission-rules")]
[Authorize(Roles = Roles.Administrador)]
public class CommissionRulesController : ControllerBase
{
    private readonly ISender _mediator;

    public CommissionRulesController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene todas las reglas de comisión
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCommissionRules(
        [FromQuery] bool? isActive,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCommissionRulesQuery(isActive), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails();
    }

    /// <summary>
    /// Crea una nueva regla de comisión
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCommissionRule(
        [FromBody] CreateCommissionRuleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCommissionRuleCommand(
            request.Name, request.RoleId, request.ZoneId, request.CategoryId,
            request.MinOrderAmount, request.MaxOrderAmount,
            request.CommissionType, request.CommissionValue,
            request.MinCommission, request.MaxCommission,
            request.Priority, request.ValidFrom, request.ValidTo);

        var result = await _mediator.Send(command, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetCommissionRules), new { }, new { id = result.Value })
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Actualiza una regla de comisión existente
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCommissionRule(
        Guid id,
        [FromBody] UpdateCommissionRuleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCommissionRuleCommand(
            id, request.Name, request.RoleId, request.ZoneId, request.CategoryId,
            request.MinOrderAmount, request.MaxOrderAmount,
            request.CommissionType, request.CommissionValue,
            request.MinCommission, request.MaxCommission,
            request.Priority, request.IsActive,
            request.ValidFrom, request.ValidTo);

        var result = await _mediator.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblemDetails();
    }

    /// <summary>
    /// Desactiva (elimina lógicamente) una regla de comisión
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCommissionRule(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteCommissionRuleCommand(id), cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblemDetails();
    }

    /// <summary>
    /// Simula qué comisión aplicaría para un escenario dado
    /// </summary>
    [HttpGet("simulate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SimulateCommission(
        [FromQuery] Guid roleId,
        [FromQuery] Guid zoneId,
        [FromQuery] Guid categoryId,
        [FromQuery] decimal orderAmount,
        CancellationToken cancellationToken)
    {
        var query = new SimulateCommissionQuery(roleId, zoneId, categoryId, orderAmount);
        var result = await _mediator.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails();
    }
}
