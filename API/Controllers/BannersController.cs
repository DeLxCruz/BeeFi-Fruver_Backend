using API.Contracts.Banners;
using API.Extensions;
using Asp.Versioning;
using Application.Features.Banners.CreateBanner;
using Application.Features.Banners.DeleteBanner;
using Application.Features.Banners.GetActiveBanners;
using Application.Features.Banners.UpdateBanner;
using Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers;

/// <summary>
/// Controlador para gestión de banners promocionales
/// </summary>
[ApiVersion(1)]
[ApiController]
[EnableRateLimiting("GlobalPolicy")]
[Route("api/v{v:apiVersion}/banners")]
public class BannersController : ControllerBase
{
    private readonly IMediator _mediator;

    public BannersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene los banners activos (público, sin autenticación)
    /// </summary>
    [HttpGet]
    [EnableRateLimiting("PublicPolicy")]
    [ProducesResponseType(typeof(List<BannerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveBanners()
    {
        var query = new GetActiveBannersQuery();
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails();
    }

    /// <summary>
    /// Crea un nuevo banner (solo Administrador)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateBanner([FromBody] CreateBannerRequest request)
    {
        var command = new CreateBannerCommand(
            request.Title, request.ImageUrl, request.LinkUrl,
            request.DisplayOrder, request.StartsAt, request.EndsAt);
        var result = await _mediator.Send(command);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetActiveBanners), new { id = result.Value }, new { bannerId = result.Value })
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Actualiza un banner existente (solo Administrador)
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBanner(Guid id, [FromBody] UpdateBannerRequest request)
    {
        var command = new UpdateBannerCommand(
            id, request.Title, request.ImageUrl, request.LinkUrl,
            request.IsActive, request.DisplayOrder, request.StartsAt, request.EndsAt);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { message = "Banner actualizado exitosamente" });

        return result.ToProblemDetails();
    }

    /// <summary>
    /// Elimina un banner definitivamente (solo Administrador)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBanner(Guid id)
    {
        var command = new DeleteBannerCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { message = "Banner eliminado exitosamente" });

        return result.ToProblemDetails();
    }
}
