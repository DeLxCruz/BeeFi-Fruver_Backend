using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Deliveries.CreateDelivery;

public class CreateDeliveryCommandHandler : IRequestHandler<CreateDeliveryCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateDeliveryCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(
        CreateDeliveryCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order is null)
            return Result.Failure<Guid>(DeliveryErrors.OrderNotEligible);

        if (order.Status is not (OrderStatus.Confirmed or OrderStatus.Preparing))
            return Result.Failure<Guid>(DeliveryErrors.OrderNotEligible);

        var existingDelivery = await _context.Deliveries
            .AnyAsync(d => d.OrderId == request.OrderId, cancellationToken);

        if (existingDelivery)
            return Result.Failure<Guid>(DeliveryErrors.OrderAlreadyHasDelivery);

        var delivery = Delivery.Create(request.OrderId, request.EstimatedDeliveryTime);
        _context.Deliveries.Add(delivery);

        var history = DeliveryStatusHistory.Create(
            deliveryId: delivery.Id,
            status: DeliveryStatus.Pending,
            updatedBy: _currentUser.UserId);
        _context.DeliveryStatusHistories.Add(history);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(delivery.Id);
    }
}
