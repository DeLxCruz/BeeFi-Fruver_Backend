using API.Contracts.Pricing;
using API.Extensions;
using Application.Features.PriceReference.GetPriceReference;
using Application.Features.PriceReference.RecomputePriceReference;
using Asp.Versioning;
using Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers;

/// <summary>
/// Precios de referencia de mercado
/// </summary>
[ApiVersion(1)]
[ApiController]
[EnableRateLimiting("GlobalPolicy")]
[Route("api/v{v:apiVersion}")]
public class PricingController : ControllerBase
{
    private readonly ISender _mediator;

    public PricingController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Consulta el precio de referencia de un producto (público)
    /// </summary>
    [HttpGet("pricing/reference")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPriceReference(
        [FromQuery] string query,
        [FromQuery] Guid? zoneId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetPriceReferenceQuery(query, zoneId), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails();
    }

    /// <summary>
    /// Recalcula los precios de referencia desde las ventas reales (solo Admin)
    /// </summary>
    [HttpPost("admin/pricing/reference/recompute")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RecomputePriceReference(
        [FromBody] RecomputePriceReferenceRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RecomputePriceReferenceCommand(request.ProductKey, request.ZoneId);
        var result = await _mediator.Send(command, cancellationToken);
        return result.IsSuccess
            ? Ok(new { recomputedCount = result.Value })
            : result.ToProblemDetails();
    }
}
