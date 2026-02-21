using Application.Common.Interfaces;
using Domain.Enums;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Result>
{
    private static readonly Dictionary<OrderStatus, OrderStatus[]> ValidTransitions = new()
    {
        [OrderStatus.Pending]   = [OrderStatus.Confirmed, OrderStatus.Cancelled],
        [OrderStatus.Confirmed] = [OrderStatus.Preparing, OrderStatus.Cancelled],
        [OrderStatus.Preparing] = [OrderStatus.InDelivery],
        [OrderStatus.InDelivery] = [OrderStatus.Delivered],
    };

    private readonly IApplicationDbContext _context;

    public UpdateOrderStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order is null)
            return Result.Failure(OrderErrors.NotFound);

        if (!ValidTransitions.TryGetValue(order.Status, out var allowed) ||
            !allowed.Contains(request.NewStatus))
        {
            return Result.Failure(OrderErrors.InvalidStatus);
        }

        order.UpdateStatus(request.NewStatus, request.Notes);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
