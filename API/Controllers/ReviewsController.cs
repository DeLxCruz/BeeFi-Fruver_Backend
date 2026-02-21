using API.Contracts.Reviews;
using API.Extensions;
using Asp.Versioning;
using Application.Features.Reviews.CreateReview;
using Application.Features.Reviews.DeleteReview;
using Application.Features.Reviews.GetFruverReviews;
using Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers;

/// <summary>
/// Controlador para reseñas de fruteros/verduleros asociados
/// </summary>
[ApiVersion(1)]
[ApiController]
[EnableRateLimiting("GlobalPolicy")]
[Route("api/v{v:apiVersion}/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene las reseñas públicas de un fruver (sin autenticación)
    /// </summary>
    [HttpGet("fruver/{fruverId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFruverReviews(
        Guid fruverId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetFruverReviewsQuery(fruverId, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails();
    }

    /// <summary>
    /// Crea una reseña para un pedido entregado
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequest request)
    {
        var command = new CreateReviewCommand(
            request.OrderId, request.FruverId, request.Rating, request.Comment);
        var result = await _mediator.Send(command);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetFruverReviews),
                new { fruverId = request.FruverId },
                new { reviewId = result.Value })
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Elimina (oculta) una reseña inapropiada (solo Administrador)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReview(Guid id)
    {
        var command = new DeleteReviewCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { message = "Reseña ocultada exitosamente" });

        return result.ToProblemDetails();
    }
}
