using API.Contracts.Loyalty;
using API.Extensions;
using Asp.Versioning;
using Application.Features.Loyalty.GetMyLoyaltyPoints;
using Application.Features.Loyalty.GetMyPointsHistory;
using Application.Features.Rewards.GetAvailableRewards;
using Application.Features.Rewards.GetMyRewards;
using Application.Features.Rewards.RedeemReward;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers;

/// <summary>
/// Controlador para puntos de lealtad y recompensas del usuario
/// </summary>
[ApiVersion(1)]
[ApiController]
[EnableRateLimiting("GlobalPolicy")]
[Route("api/v{v:apiVersion}/loyalty")]
[Authorize]
public class LoyaltyController : ControllerBase
{
    private readonly IMediator _mediator;

    public LoyaltyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene los puntos de lealtad del usuario autenticado
    /// </summary>
    [HttpGet("points")]
    [ProducesResponseType(typeof(LoyaltyPointsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyLoyaltyPoints()
    {
        var query = new GetMyLoyaltyPointsQuery();
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails();
    }

    /// <summary>
    /// Obtiene el historial de transacciones de puntos
    /// </summary>
    [HttpGet("points/history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyPointsHistory(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetMyPointsHistoryQuery(pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails();
    }

    /// <summary>
    /// Obtiene las recompensas disponibles (no requiere autenticación)
    /// </summary>
    [HttpGet("rewards")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableRewards(
        [FromQuery] RewardType? type = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetAvailableRewardsQuery(type, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails();
    }

    /// <summary>
    /// Obtiene las recompensas canjeadas por el usuario autenticado
    /// </summary>
    [HttpGet("rewards/mine")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyRewards(
        [FromQuery] RewardStatus? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetMyRewardsQuery(status, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails();
    }

    /// <summary>
    /// Canjea una recompensa con puntos de lealtad
    /// </summary>
    [HttpPost("rewards/redeem")]
    [ProducesResponseType(typeof(RedeemRewardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RedeemReward([FromBody] RedeemRewardRequest request)
    {
        var command = new RedeemRewardCommand(request.RewardId);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails();
    }
}
