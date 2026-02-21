using API.Contracts.Payments;
using API.Extensions;
using Asp.Versioning;
using Application.Features.Payments.ConfirmPayment;
using Application.Features.Payments.GetAllPayments;
using Application.Features.Payments.GetPaymentByOrder;
using Application.Features.Payments.InitiatePayment;
using Application.Features.Payments.ProcessRefund;
using Domain.Constants;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers;

/// <summary>
/// Controlador para gestión de pagos
/// </summary>
[ApiVersion(1)]
[ApiController]
[EnableRateLimiting("PublicPolicy")]
[Route("api/v{v:apiVersion}/payments")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene el pago asociado a un pedido
    /// </summary>
    [HttpGet("order/{orderId:guid}")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentByOrder(Guid orderId)
    {
        var query = new GetPaymentByOrderQuery(orderId);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return Ok(result.Value);

        return result.ToProblemDetails();
    }

    /// <summary>
    /// Obtiene todos los pagos (solo Administrador)
    /// </summary>
    [HttpGet("admin/all")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllPayments(
        [FromQuery] PaymentStatus? status = null,
        [FromQuery] PaymentMethod? method = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetAllPaymentsQuery(status, method, fromDate, toDate, pageNumber, pageSize);
        var result = await _mediator.Send(query);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails();
    }

    /// <summary>
    /// Inicia el proceso de pago para un pedido
    /// </summary>
    [HttpPost("initiate")]
    [ProducesResponseType(typeof(InitiatePaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> InitiatePayment([FromBody] InitiatePaymentRequest request)
    {
        var command = new InitiatePaymentCommand(request.OrderId, request.PaymentMethod, request.ReturnUrl);
        var result = await _mediator.Send(command);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails();
    }

    /// <summary>
    /// Confirma un pago (solo Administrador)
    /// </summary>
    [HttpPost("confirm")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentRequest request)
    {
        var command = new ConfirmPaymentCommand(request.OrderId, request.TransactionId, request.GatewayResponse);
        var result = await _mediator.Send(command);

        return result.IsSuccess
            ? Ok(new { message = "Pago confirmado exitosamente" })
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Webhook para confirmación de pago desde pasarela externa
    /// </summary>
    [HttpPost("webhook")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PaymentWebhook([FromBody] ConfirmPaymentRequest request)
    {
        var command = new ConfirmPaymentCommand(request.OrderId, request.TransactionId, request.GatewayResponse);
        var result = await _mediator.Send(command);

        return result.IsSuccess ? Ok() : result.ToProblemDetails();
    }

    /// <summary>
    /// Procesa un reembolso (solo Administrador)
    /// </summary>
    [HttpPost("refund")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ProcessRefund([FromBody] ProcessRefundRequest request)
    {
        var command = new ProcessRefundCommand(request.OrderId, request.Amount, request.Reason);
        var result = await _mediator.Send(command);

        return result.IsSuccess
            ? Ok(new { message = "Reembolso procesado exitosamente" })
            : result.ToProblemDetails();
    }
}
