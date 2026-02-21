using API.Contracts.Cart;
using API.Extensions;
using Asp.Versioning;
using Application.Features.Cart.AddToCart;
using Application.Features.Cart.ClearCart;
using Application.Features.Cart.GetCart;
using Application.Features.Cart.RemoveFromCart;
using Application.Features.Cart.UpdateCartItem;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers;

/// <summary>
/// Controlador para gestión del carrito de compras
/// </summary>
[ApiVersion(1)]
[ApiController]
[EnableRateLimiting("GlobalPolicy")]
[Route("api/v{v:apiVersion}/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;

    public CartController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene el carrito del usuario autenticado
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCart()
    {
        var result = await _mediator.Send(new GetCartQuery());
        return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails();
    }

    /// <summary>
    /// Agrega un producto al carrito
    /// </summary>
    [HttpPost("items")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        var command = new AddToCartCommand(request.FruverProductId, request.Quantity);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(new { message = "Producto agregado al carrito" }) : result.ToProblemDetails();
    }

    /// <summary>
    /// Actualiza la cantidad de un item del carrito
    /// </summary>
    [HttpPut("items/{cartItemId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCartItem(Guid cartItemId, [FromBody] UpdateCartItemRequest request)
    {
        var command = new UpdateCartItemCommand(cartItemId, request.Quantity);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { message = "Cantidad actualizada" });

        return result.ToProblemDetails();
    }

    /// <summary>
    /// Elimina un item del carrito
    /// </summary>
    [HttpDelete("items/{cartItemId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFromCart(Guid cartItemId)
    {
        var command = new RemoveFromCartCommand(cartItemId);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { message = "Producto eliminado del carrito" });

        return result.ToProblemDetails();
    }

    /// <summary>
    /// Vacía el carrito completo del usuario
    /// </summary>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ClearCart()
    {
        var result = await _mediator.Send(new ClearCartCommand());
        return result.IsSuccess ? Ok(new { message = "Carrito vaciado" }) : result.ToProblemDetails();
    }
}
