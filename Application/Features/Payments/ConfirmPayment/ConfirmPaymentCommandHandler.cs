using Application.Common.Interfaces;
using Domain.Enums;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payments.ConfirmPayment;

public class ConfirmPaymentCommandHandler : IRequestHandler<ConfirmPaymentCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public ConfirmPaymentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.OrderId == request.OrderId, cancellationToken);

        if (payment is null)
            return Result.Failure(PaymentErrors.NotFound);

        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order is null)
            return Result.Failure(PaymentErrors.OrderNotFound);

        payment.Complete(request.GatewayResponse);
        order.UpdatePaymentStatus(PaymentStatus.Completed);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
