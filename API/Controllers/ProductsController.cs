using API.Contracts.Common;
using API.Contracts.Products;
using Domain.Constants;
using Application.Common.Models;
using Application.Features.Products.AddProductImage;
using Application.Features.Products.CreateProduct;
using Application.Features.Products.DeleteProduct;
using Application.Features.Products.DeleteProductImage;
using Application.Features.Products.GetProductById;
using Application.Features.Products.GetProducts;
using Application.Features.Products.GetProductsByCategory;
using Application.Features.Products.UpdateProduct;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Catálogo base de productos
/// </summary>
[ApiController]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ISender mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene el catálogo de productos con filtros y paginación
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaginatedList<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(
        [FromQuery] Guid? categoryId,
        [FromQuery] string? searchTerm,
        [FromQuery] bool? isActive,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductsQuery(categoryId, searchTerm, isActive ?? true, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result.Value);
    }

    /// <summary>
    /// Obtiene un producto por su Id
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id), cancellationToken);

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
    /// Obtiene productos por categoría, incluyendo subcategorías
    /// </summary>
    [HttpGet("category/{categoryId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaginatedList<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductsByCategory(
        Guid categoryId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductsByCategoryQuery(categoryId, pageNumber, pageSize);
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
    /// Crea un nuevo producto en el catálogo base
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(CreateProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(
            request.Name,
            request.Description,
            request.CategoryId,
            request.ImageUrl,
            request.UnitOfMeasure);

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
                "Category.NotFound" => NotFound(errorResponse),
                "Product.AlreadyExists" => Conflict(errorResponse),
                _ => BadRequest(errorResponse)
            };
        }

        return CreatedAtAction(
            nameof(GetProductById),
            new { id = result.Value.Id },
            result.Value);
    }

    /// <summary>
    /// Actualiza un producto existente
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(
        Guid id,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(
            id,
            request.Name,
            request.Description,
            request.CategoryId,
            request.ImageUrl,
            request.UnitOfMeasure,
            request.IsActive);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            var errorResponse = new ErrorResponse(
                code: result.Error.Code,
                message: result.Error.Message,
                traceId: HttpContext.TraceIdentifier,
                path: HttpContext.Request.Path);

            return result.Error.Code is "Product.NotFound" or "Category.NotFound"
                ? NotFound(errorResponse)
                : BadRequest(errorResponse);
        }

        return NoContent();
    }

    /// <summary>
    /// Desactiva un producto (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteProductCommand(id), cancellationToken);

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
                "Product.HasActiveFruverProducts" => Conflict(errorResponse),
                _ => BadRequest(errorResponse)
            };
        }

        return NoContent();
    }

    /// <summary>
    /// Agrega una imagen a un producto
    /// </summary>
    [HttpPost("{id:guid}/images")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddProductImage(
        Guid id,
        [FromBody] AddProductImageRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddProductImageCommand(id, request.ImageUrl, request.DisplayOrder);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            var errorResponse = new ErrorResponse(
                code: result.Error.Code,
                message: result.Error.Message,
                traceId: HttpContext.TraceIdentifier,
                path: HttpContext.Request.Path);

            return result.Error.Code == "Product.NotFound"
                ? NotFound(errorResponse)
                : BadRequest(errorResponse);
        }

        return NoContent();
    }

    /// <summary>
    /// Elimina una imagen de producto
    /// </summary>
    [HttpDelete("images/{imageId:guid}")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProductImage(Guid imageId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteProductImageCommand(imageId), cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(new ErrorResponse(
                code: result.Error.Code,
                message: result.Error.Message,
                traceId: HttpContext.TraceIdentifier,
                path: HttpContext.Request.Path));
        }

        return NoContent();
    }
}
