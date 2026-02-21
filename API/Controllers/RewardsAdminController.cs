using API.Contracts.Rewards;
using API.Extensions;
using Asp.Versioning;
using Application.Features.Rewards.CreateReward;
using Application.Features.Rewards.UpdateReward;
using Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers;

/// <summary>
/// Controlador de administración de recompensas
/// </summary>
[ApiVersion(1)]
[ApiController]
[EnableRateLimiting("GlobalPolicy")]
[Route("api/v{v:apiVersion}/admin/rewards")]
[Authorize(Roles = Roles.Administrador)]
public class RewardsAdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public RewardsAdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Crea una nueva recompensa
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateReward([FromBody] CreateRewardRequest request)
    {
        var command = new CreateRewardCommand(
            request.Name, request.Description, request.ImageUrl,
            request.PointsRequired, request.Type, request.Value,
            request.IsExclusive, request.MaxRedemptionsPerUser, request.ExpirationDate);
        var result = await _mediator.Send(command);

        return result.IsSuccess
            ? CreatedAtAction(nameof(CreateReward), new { id = result.Value }, new { rewardId = result.Value })
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Actualiza una recompensa existente
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateReward(Guid id, [FromBody] UpdateRewardRequest request)
    {
        var command = new UpdateRewardCommand(
            id, request.Name, request.Description, request.ImageUrl,
            request.PointsRequired, request.Value, request.IsActive,
            request.MaxRedemptionsPerUser, request.ExpirationDate);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { message = "Recompensa actualizada exitosamente" });

        return result.ToProblemDetails();
    }
}
