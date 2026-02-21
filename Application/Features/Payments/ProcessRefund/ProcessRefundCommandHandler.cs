using Application.Common.Interfaces;
using Domain.Enums;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payments.ProcessRefund;

public class ProcessRefundCommandHandler : IRequestHandler<ProcessRefundCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentGateway _paymentGateway;

    public ProcessRefundCommandHandler(
        IApplicationDbContext context,
        IPaymentGateway paymentGateway)
    {
        _context = context;
        _paymentGateway = paymentGateway;
    }

    public async Task<Result> Handle(ProcessRefundCommand request, CancellationToken cancellationToken)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.OrderId == request.OrderId, cancellationToken);

        if (payment is null)
            return Result.Failure(PaymentErrors.NotFound);

        if (payment.Status != PaymentStatus.Completed)
            return Result.Failure(PaymentErrors.CannotRefund);

        if (payment.Method != PaymentMethod.CashOnDelivery)
        {
            var refundResult = await _paymentGateway.RefundPaymentAsync(
                payment.TransactionId!,
                request.Amount,
                request.Reason,
                cancellationToken);

            if (!refundResult.IsSuccess)
                return Result.Failure(PaymentErrors.GatewayError);
        }

        payment.Refund(request.Amount, request.Reason);

        // If full refund, cancel the order
        if (request.Amount >= payment.Amount)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

            order?.UpdateStatus(OrderStatus.Cancelled, $"Reembolso: {request.Reason}");
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
