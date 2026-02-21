using API.Contracts.Orders;
using Application.Features.Orders.CancelOrder;
using Application.Features.Orders.CreateOrder;
using Application.Features.Orders.GetAllOrders;
using Application.Features.Orders.GetMyOrders;
using Application.Features.Orders.GetOrderById;
using Application.Features.Orders.UpdateOrderStatus;
using Domain.Constants;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controlador para gestión de pedidos
/// </summary>
[ApiController]
[Route("api/v1/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene los pedidos del usuario autenticado
    /// </summary>
    [HttpGet("my")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetMyOrdersQuery(pageNumber, pageSize));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Obtiene todos los pedidos (solo admin o empleado)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = Roles.AdminOrEmpleado)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllOrders(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] OrderStatus? status = null,
        [FromQuery] string? search = null)
    {
        var result = await _mediator.Send(new GetAllOrdersQuery(pageNumber, pageSize, status, search));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Obtiene el detalle de un pedido por ID
    /// </summary>
    [HttpGet("{orderId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetOrderById(Guid orderId)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(orderId));

        if (result.IsSuccess)
            return Ok(result.Value);

        return result.Error.Code switch
        {
            "Order.NotFound" => NotFound(result.Error),
            "Order.NotOwner" => Forbid(),
            _ => BadRequest(result.Error)
        };
    }

    /// <summary>
    /// Crea un nuevo pedido a partir del carrito
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var command = new CreateOrderCommand(request.AddressId, request.PaymentMethod, request.Notes);
        var result = await _mediator.Send(command);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetOrderById), new { orderId = result.Value }, new { id = result.Value })
            : BadRequest(result.Error);
    }

    /// <summary>
    /// Cancela un pedido propio
    /// </summary>
    [HttpPost("{orderId:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelOrder(Guid orderId, [FromBody] CancelOrderRequest request)
    {
        var command = new CancelOrderCommand(orderId, request.Reason);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { message = "Pedido cancelado exitosamente" });

        return result.Error.Code switch
        {
            "Order.NotFound" => NotFound(result.Error),
            _ => BadRequest(result.Error)
        };
    }

    /// <summary>
    /// Actualiza el estado de un pedido (solo admin o empleado)
    /// </summary>
    [HttpPatch("{orderId:guid}/status")]
    [Authorize(Roles = Roles.AdminOrEmpleado)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrderStatus(Guid orderId, [FromBody] UpdateOrderStatusRequest request)
    {
        var command = new UpdateOrderStatusCommand(orderId, request.NewStatus, request.Notes);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { message = "Estado del pedido actualizado" });

        return result.Error.Code switch
        {
            "Order.NotFound" => NotFound(result.Error),
            _ => BadRequest(result.Error)
        };
    }
}
