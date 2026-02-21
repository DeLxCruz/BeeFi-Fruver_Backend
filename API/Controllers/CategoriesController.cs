using API.Contracts.Categories;
using API.Contracts.Common;
using Domain.Constants;
using Application.Features.Categories.CreateCategory;
using Application.Features.Categories.DeleteCategory;
using Application.Features.Categories.GetCategories;
using Application.Features.Categories.GetCategoryById;
using Application.Features.Categories.UpdateCategory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Gestión de categorías de productos
/// </summary>
[ApiController]
[Route("api/v1/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ISender mediator, ILogger<CategoriesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las categorías activas en estructura de árbol
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCategoriesQuery(), cancellationToken);
        return Ok(result.Value);
    }

    /// <summary>
    /// Obtiene una categoría por su Id
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategoryById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCategoryByIdQuery(id), cancellationToken);

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
    /// Crea una nueva categoría
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(CreateCategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCategory(
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand(
            request.Name,
            request.Description,
            request.ImageUrl,
            request.ParentCategoryId,
            request.DisplayOrder);

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
                "Category.AlreadyExists" => Conflict(errorResponse),
                "Category.ParentNotFound" => NotFound(errorResponse),
                _ => BadRequest(errorResponse)
            };
        }

        return CreatedAtAction(
            nameof(GetCategoryById),
            new { id = result.Value.Id },
            result.Value);
    }

    /// <summary>
    /// Actualiza una categoría existente
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCategory(
        Guid id,
        [FromBody] UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCategoryCommand(
            id,
            request.Name,
            request.Description,
            request.ImageUrl,
            request.IsActive,
            request.DisplayOrder);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            var errorResponse = new ErrorResponse(
                code: result.Error.Code,
                message: result.Error.Message,
                traceId: HttpContext.TraceIdentifier,
                path: HttpContext.Request.Path);

            return result.Error.Code == "Category.NotFound"
                ? NotFound(errorResponse)
                : BadRequest(errorResponse);
        }

        return NoContent();
    }

    /// <summary>
    /// Desactiva (elimina lógicamente) una categoría
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteCategoryCommand(id), cancellationToken);

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
                "Category.HasActiveProducts" => Conflict(errorResponse),
                _ => BadRequest(errorResponse)
            };
        }

        return NoContent();
    }
}
