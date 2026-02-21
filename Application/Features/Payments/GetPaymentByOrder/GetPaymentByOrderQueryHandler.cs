using Application.Common.Interfaces;
using Domain.Constants;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payments.GetPaymentByOrder;

public class GetPaymentByOrderQueryHandler
    : IRequestHandler<GetPaymentByOrderQuery, Result<PaymentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetPaymentByOrderQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PaymentDto>> Handle(
        GetPaymentByOrderQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;
        var isAdminOrEmpleado = _currentUser.Roles.Contains(Roles.Administrador)
            || _currentUser.Roles.Contains(Roles.Empleado);

        var payment = await _context.Payments
            .AsNoTracking()
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.OrderId == request.OrderId, cancellationToken);

        if (payment is null)
            return Result.Failure<PaymentDto>(PaymentErrors.NotFound);

        if (!isAdminOrEmpleado && payment.Order.UserId != userId)
            return Result.Failure<PaymentDto>(PaymentErrors.NotFound);

        return Result.Success(new PaymentDto(
            payment.Id,
            payment.OrderId,
            payment.Order.OrderNumber,
            payment.Method,
            payment.Status,
            payment.Amount,
            payment.TransactionId,
            payment.GatewayResponse,
            payment.RefundAmount,
            payment.RefundedAt,
            payment.RefundReason,
            payment.CreatedAt));
    }
}
