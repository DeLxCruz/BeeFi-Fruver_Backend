using Application.Common.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Deliveries.AssignDeliveryPerson;

public class AssignDeliveryPersonCommandHandler : IRequestHandler<AssignDeliveryPersonCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AssignDeliveryPersonCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        AssignDeliveryPersonCommand request,
        CancellationToken cancellationToken)
    {
        var delivery = await _context.Deliveries
            .Include(d => d.Order)
                .ThenInclude(o => o.Address)
            .FirstOrDefaultAsync(d => d.Id == request.DeliveryId, cancellationToken);

        if (delivery is null)
            return Result.Failure(DeliveryErrors.NotFound);

        if (delivery.DeliveryPersonId is not null)
            return Result.Failure(DeliveryErrors.AlreadyAssigned);

        // Verify delivery person exists and has Empleado role
        var personHasRole = await _context.UserRoles
            .Include(ur => ur.Role)
            .AnyAsync(ur =>
                ur.UserId == request.DeliveryPersonId &&
                ur.Role.Name == Roles.Empleado,
                cancellationToken);

        if (!personHasRole)
            return Result.Failure(DeliveryErrors.DeliveryPersonNotFound);

        // Verify delivery person is assigned to the order's zone
        var zoneId = delivery.Order.Address.ZoneId;
        if (zoneId != Guid.Empty)
        {
            var inZone = await _context.DeliveryPersonZones
                .AnyAsync(dpz =>
                    dpz.DeliveryPersonId == request.DeliveryPersonId &&
                    dpz.ZoneId == zoneId,
                    cancellationToken);

            if (!inZone)
                return Result.Failure(DeliveryErrors.DeliveryPersonNotInZone);
        }

        delivery.AssignDeliveryPerson(request.DeliveryPersonId);

        var history = DeliveryStatusHistory.Create(
            deliveryId: delivery.Id,
            status: DeliveryStatus.Assigned,
            updatedBy: _currentUser.UserId,
            notes: $"Repartidor asignado");
        _context.DeliveryStatusHistories.Add(history);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
