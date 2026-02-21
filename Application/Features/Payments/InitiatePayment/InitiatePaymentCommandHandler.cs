using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payments.InitiatePayment;

public class InitiatePaymentCommandHandler
    : IRequestHandler<InitiatePaymentCommand, Result<InitiatePaymentResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IPaymentGateway _paymentGateway;

    public InitiatePaymentCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IPaymentGateway paymentGateway)
    {
        _context = context;
        _currentUser = currentUser;
        _paymentGateway = paymentGateway;
    }

    public async Task<Result<InitiatePaymentResponse>> Handle(
        InitiatePaymentCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        var order = await _context.Orders
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order is null)
            return Result.Failure<InitiatePaymentResponse>(PaymentErrors.OrderNotFound);

        if (order.UserId != userId)
            return Result.Failure<InitiatePaymentResponse>(PaymentErrors.OrderNotFound);

        // Check if already paid
        var existingPayment = await _context.Payments
            .FirstOrDefaultAsync(p => p.OrderId == request.OrderId, cancellationToken);

        if (existingPayment is not null && existingPayment.Status == PaymentStatus.Completed)
            return Result.Failure<InitiatePaymentResponse>(PaymentErrors.AlreadyPaid);

        var payment = Payment.Create(request.OrderId, request.PaymentMethod, order.Total);

        if (request.PaymentMethod == PaymentMethod.CashOnDelivery)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(new InitiatePaymentResponse(
                payment.Id,
                IsCashOnDelivery: true,
                RedirectUrl: null,
                TransactionId: null));
        }

        // Electronic payment: call gateway
        var gatewayRequest = new PaymentInitRequest
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            Amount = order.Total,
            PaymentMethod = request.PaymentMethod,
            UserEmail = order.User.Email,
            UserName = $"{order.User.FirstName} {order.User.LastName}",
            Description = $"Pedido BeeFi {order.OrderNumber}",
            ReturnUrl = request.ReturnUrl
        };

        var gatewayResult = await _paymentGateway.InitiatePaymentAsync(gatewayRequest, cancellationToken);

        if (!gatewayResult.IsSuccess)
            return Result.Failure<InitiatePaymentResponse>(PaymentErrors.GatewayError);

        payment.MarkAsProcessing(gatewayResult.TransactionId!);
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(new InitiatePaymentResponse(
            payment.Id,
            IsCashOnDelivery: false,
            RedirectUrl: gatewayResult.RedirectUrl,
            TransactionId: gatewayResult.TransactionId));
    }
}
