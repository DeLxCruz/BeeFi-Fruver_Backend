using Application.Common.Interfaces;
using Application.Features.Loyalty.EarnPoints;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Deliveries.UpdateDeliveryStatus;

public class UpdateDeliveryStatusCommandHandler : IRequestHandler<UpdateDeliveryStatusCommand, Result>
{
    private static readonly Dictionary<DeliveryStatus, DeliveryStatus[]> ValidTransitions = new()
    {
        [DeliveryStatus.Assigned]         = [DeliveryStatus.PickedUp],
        [DeliveryStatus.PickedUp]         = [DeliveryStatus.OnRoute],
        [DeliveryStatus.OnRoute]          = [DeliveryStatus.Delivered, DeliveryStatus.Failed],
    };

    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ISender _sender;

    public UpdateDeliveryStatusCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        ISender sender)
    {
        _context = context;
        _currentUser = currentUser;
        _sender = sender;
    }

    public async Task<Result> Handle(
        UpdateDeliveryStatusCommand request,
        CancellationToken cancellationToken)
    {
        var delivery = await _context.Deliveries
            .Include(d => d.Order)
            .FirstOrDefaultAsync(d => d.Id == request.DeliveryId, cancellationToken);

        if (delivery is null)
            return Result.Failure(DeliveryErrors.NotFound);

        if (delivery.DeliveryPersonId != _currentUser.UserId)
            return Result.Failure(DeliveryErrors.NotOwner);

        if (!ValidTransitions.TryGetValue(delivery.Status, out var allowed) ||
            !allowed.Contains(request.NewStatus))
            return Result.Failure(DeliveryErrors.InvalidTransition);

        delivery.UpdateStatus(request.NewStatus, request.Notes);

        var history = DeliveryStatusHistory.Create(
            deliveryId: delivery.Id,
            status: request.NewStatus,
            updatedBy: _currentUser.UserId,
            notes: request.Notes,
            latitude: request.Latitude,
            longitude: request.Longitude);
        _context.DeliveryStatusHistories.Add(history);

        if (request.NewStatus == DeliveryStatus.Delivered)
        {
            delivery.Order.Complete();

            if (delivery.Order.PaymentMethod == PaymentMethod.CashOnDelivery)
            {
                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.OrderId == delivery.OrderId, cancellationToken);

                if (payment is not null && payment.Status == PaymentStatus.Pending)
                {
                    payment.Complete(null);
                    delivery.Order.UpdatePaymentStatus(PaymentStatus.Completed);
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Acumular puntos de lealtad al entregar el pedido
        if (request.NewStatus == DeliveryStatus.Delivered)
        {
            await _sender.Send(new EarnPointsCommand(
                delivery.Order.UserId,
                delivery.OrderId,
                delivery.Order.Total), cancellationToken);
        }

        return Result.Success();
    }
}
