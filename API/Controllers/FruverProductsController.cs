using API.Contracts.FruverProducts;
using API.Extensions;
using Asp.Versioning;
using Domain.Constants;
using Application.Common.Models;
using Application.Features.FruverProducts.GetCatalogByZone;
using Application.Features.FruverProducts.GetFruverProductById;
using Application.Features.FruverProducts.GetFruverProducts;
using Application.Features.FruverProducts.AddVariant;
using Application.Features.FruverProducts.DeleteVariant;
using Application.Features.FruverProducts.UpdateVariant;
using Application.Features.FruverProducts.PublishFruverProduct;
using Application.Features.FruverProducts.UnpublishFruverProduct;
using Application.Features.FruverProducts.UpdateFruverProduct;
using Application.Features.FruverProducts.UpdateStock;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers;

/// <summary>
/// Catálogo de productos publicados por fruvers
/// </summary>
[ApiVersion(1)]
[ApiController]
[EnableRateLimiting("GlobalPolicy")]
[Route("api/v{v:apiVersion}/fruver-products")]
public class FruverProductsController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly ILogger<FruverProductsController> _logger;

    public FruverProductsController(ISender mediator, ILogger<FruverProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene los productos publicados con filtros y paginación
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaginatedList<FruverProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFruverProducts(
        [FromQuery] Guid fruverId,
        [FromQuery] Guid? zoneId,
        [FromQuery] Guid? categoryId,
        [FromQuery] string? searchTerm,
        [FromQuery] bool inStockOnly = true,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetFruverProductsQuery(
            fruverId, zoneId, categoryId, searchTerm, inStockOnly, pageNumber, pageSize);

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result.Value);
    }

    /// <summary>
    /// Obtiene un producto de fruver por su Id
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(FruverProductDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFruverProductById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetFruverProductByIdQuery(id), cancellationToken);

        if (result.IsFailure)
            return result.ToProblemDetails();

        return Ok(result.Value);
    }

    /// <summary>
    /// Catálogo completo de una zona — endpoint estrella
    /// </summary>
    [HttpGet("catalog/zone/{zoneId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaginatedList<ZoneCatalogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCatalogByZone(
        Guid zoneId,
        [FromQuery] Guid? categoryId,
        [FromQuery] string? searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCatalogByZoneQuery(zoneId, categoryId, searchTerm, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return result.ToProblemDetails();

        return Ok(result.Value);
    }

    /// <summary>
    /// El fruver publica un producto del catálogo base
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Roles.FruverAliado)]
    [ProducesResponseType(typeof(PublishFruverProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> PublishFruverProduct(
        [FromBody] PublishFruverProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = new PublishFruverProductCommand(
            request.ProductId,
            request.Price,
            request.Stock,
            request.DiscountPercentage,
            request.BeeFiExclusiveDiscount);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.ToProblemDetails();

        return CreatedAtAction(
            nameof(GetFruverProductById),
            new { id = result.Value.Id },
            result.Value);
    }

    /// <summary>
    /// El fruver actualiza precio/stock/descuentos de su producto publicado
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.FruverAliado)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateFruverProduct(
        Guid id,
        [FromBody] UpdateFruverProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateFruverProductCommand(
            id,
            request.Price,
            request.Stock,
            request.DiscountPercentage,
            request.BeeFiExclusiveDiscount,
            request.IsAvailable);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.ToProblemDetails();

        return NoContent();
    }

    /// <summary>
    /// El fruver despublica su producto (soft unpublish)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.FruverAliado)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnpublishFruverProduct(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UnpublishFruverProductCommand(id), cancellationToken);

        if (result.IsFailure)
            return result.ToProblemDetails();

        return NoContent();
    }

    /// <summary>
    /// Actualización rápida de stock
    /// </summary>
    [HttpPatch("{id:guid}/stock")]
    [Authorize(Roles = Roles.FruverAliado)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStock(
        Guid id,
        [FromBody] UpdateStockRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateStockCommand(id, request.NewStock);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.ToProblemDetails();

        return NoContent();
    }

    /// <summary>
    /// Añade una variante (tamaño, peso, presentación) a un producto publicado
    /// </summary>
    [HttpPost("{id:guid}/variants")]
    [Authorize(Roles = Roles.FruverAliado)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddVariant(
        Guid id,
        [FromBody] AddVariantRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddVariantCommand(
            id, request.Name, request.PriceAdjustment, request.Stock,
            request.DisplayOrder, request.SKU);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.ToProblemDetails();

        return CreatedAtAction(nameof(GetFruverProductById), new { id }, new { variantId = result.Value });
    }

    /// <summary>
    /// Actualiza una variante de un producto publicado
    /// </summary>
    [HttpPut("{id:guid}/variants/{variantId:guid}")]
    [Authorize(Roles = Roles.FruverAliado)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVariant(
        Guid id,
        Guid variantId,
        [FromBody] UpdateVariantRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateVariantCommand(
            variantId, request.Name, request.PriceAdjustment, request.Stock,
            request.IsActive, request.DisplayOrder, request.SKU);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.ToProblemDetails();

        return NoContent();
    }

    /// <summary>
    /// Desactiva (elimina lógicamente) una variante
    /// </summary>
    [HttpDelete("{id:guid}/variants/{variantId:guid}")]
    [Authorize(Roles = Roles.FruverAliado)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteVariant(
        Guid id,
        Guid variantId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteVariantCommand(variantId), cancellationToken);

        if (result.IsFailure)
            return result.ToProblemDetails();

        return NoContent();
    }
}
