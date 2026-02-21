using Application.Common.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Deliveries.AssignDeliveryPersonToZone;

public class AssignDeliveryPersonToZoneCommandHandler : IRequestHandler<AssignDeliveryPersonToZoneCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public AssignDeliveryPersonToZoneCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        AssignDeliveryPersonToZoneCommand request,
        CancellationToken cancellationToken)
    {
        // Verify delivery person exists and has Empleado role
        var personHasRole = await _context.UserRoles
            .Include(ur => ur.Role)
            .AnyAsync(ur =>
                ur.UserId == request.DeliveryPersonId &&
                ur.Role.Name == Roles.Empleado,
                cancellationToken);

        if (!personHasRole)
            return Result.Failure(DeliveryErrors.DeliveryPersonNotFound);

        // Verify zone exists and is active
        var zone = await _context.Zones
            .FirstOrDefaultAsync(z => z.Id == request.ZoneId && z.IsActive, cancellationToken);

        if (zone is null)
            return Result.Failure(DeliveryErrors.ZoneNotFound);

        // Check not already assigned
        var alreadyAssigned = await _context.DeliveryPersonZones
            .AnyAsync(dpz =>
                dpz.DeliveryPersonId == request.DeliveryPersonId &&
                dpz.ZoneId == request.ZoneId,
                cancellationToken);

        if (alreadyAssigned)
            return Result.Failure(DeliveryErrors.AlreadyInZone);

        var assignment = DeliveryPersonZone.Create(request.DeliveryPersonId, request.ZoneId);
        _context.DeliveryPersonZones.Add(assignment);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
