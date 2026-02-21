using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CancelOrderCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        var order = await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.FruverProduct)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order is null)
            return Result.Failure(OrderErrors.NotFound);

        if (order.UserId != userId)
            return Result.Failure(OrderErrors.NotOwner);

        if (!order.CanBeCancelled)
            return Result.Failure(OrderErrors.CannotCancel);

        order.Cancel(request.Reason);

        // Restore stock for each item
        foreach (var item in order.Items)
        {
            item.FruverProduct.RestoreStock(item.Quantity);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
