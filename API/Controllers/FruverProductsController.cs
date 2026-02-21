using API.Contracts.Common;
using API.Contracts.FruverProducts;
using Domain.Constants;
using Application.Common.Models;
using Application.Features.FruverProducts.GetCatalogByZone;
using Application.Features.FruverProducts.GetFruverProductById;
using Application.Features.FruverProducts.GetFruverProducts;
using Application.Features.FruverProducts.PublishFruverProduct;
using Application.Features.FruverProducts.UnpublishFruverProduct;
using Application.Features.FruverProducts.UpdateFruverProduct;
using Application.Features.FruverProducts.UpdateStock;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Catálogo de productos publicados por fruvers
/// </summary>
[ApiController]
[Route("api/v1/fruver-products")]
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
    [ProducesResponseType(typeof(FruverProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFruverProductById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetFruverProductByIdQuery(id), cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(new ErrorResponse(
                code: result.Error.Code,
                message: result.Error.Message,
                traceId: HttpContext.TraceIdentifier,
                path: HttpContext.Request.Path));
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Catálogo completo de una zona — endpoint estrella
    /// </summary>
    [HttpGet("catalog/zone/{zoneId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaginatedList<ZoneCatalogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
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
        {
            return NotFound(new ErrorResponse(
                code: result.Error.Code,
                message: result.Error.Message,
                traceId: HttpContext.TraceIdentifier,
                path: HttpContext.Request.Path));
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// El fruver publica un producto del catálogo base
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Roles.FruverAliado)]
    [ProducesResponseType(typeof(PublishFruverProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
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
        {
            var errorResponse = new ErrorResponse(
                code: result.Error.Code,
                message: result.Error.Message,
                traceId: HttpContext.TraceIdentifier,
                path: HttpContext.Request.Path);

            return result.Error.Code switch
            {
                "Product.NotFound" => NotFound(errorResponse),
                "FruverProduct.AlreadyExists" => Conflict(errorResponse),
                _ => BadRequest(errorResponse)
            };
        }

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
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
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
        {
            var errorResponse = new ErrorResponse(
                code: result.Error.Code,
                message: result.Error.Message,
                traceId: HttpContext.TraceIdentifier,
                path: HttpContext.Request.Path);

            return result.Error.Code switch
            {
                "FruverProduct.NotFound" => NotFound(errorResponse),
                "FruverProduct.NotOwner" => Forbid(),
                _ => BadRequest(errorResponse)
            };
        }

        return NoContent();
    }

    /// <summary>
    /// El fruver despublica su producto (soft unpublish)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.FruverAliado)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnpublishFruverProduct(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UnpublishFruverProductCommand(id), cancellationToken);

        if (result.IsFailure)
        {
            var errorResponse = new ErrorResponse(
                code: result.Error.Code,
                message: result.Error.Message,
                traceId: HttpContext.TraceIdentifier,
                path: HttpContext.Request.Path);

            return result.Error.Code switch
            {
                "FruverProduct.NotFound" => NotFound(errorResponse),
                "FruverProduct.NotOwner" => Forbid(),
                _ => BadRequest(errorResponse)
            };
        }

        return NoContent();
    }

    /// <summary>
    /// Actualización rápida de stock
    /// </summary>
    [HttpPatch("{id:guid}/stock")]
    [Authorize(Roles = Roles.FruverAliado)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStock(
        Guid id,
        [FromBody] UpdateStockRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateStockCommand(id, request.NewStock);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            var errorResponse = new ErrorResponse(
                code: result.Error.Code,
                message: result.Error.Message,
                traceId: HttpContext.TraceIdentifier,
                path: HttpContext.Request.Path);

            return result.Error.Code switch
            {
                "FruverProduct.NotFound" => NotFound(errorResponse),
                "FruverProduct.NotOwner" => Forbid(),
                _ => BadRequest(errorResponse)
            };
        }

        return NoContent();
    }
}
