using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Deliveries.RemoveDeliveryPersonFromZone;

public class RemoveDeliveryPersonFromZoneCommandHandler : IRequestHandler<RemoveDeliveryPersonFromZoneCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public RemoveDeliveryPersonFromZoneCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        RemoveDeliveryPersonFromZoneCommand request,
        CancellationToken cancellationToken)
    {
        var assignment = await _context.DeliveryPersonZones
            .FirstOrDefaultAsync(dpz =>
                dpz.DeliveryPersonId == request.DeliveryPersonId &&
                dpz.ZoneId == request.ZoneId,
                cancellationToken);

        if (assignment is null)
            return Result.Failure(DeliveryErrors.ZoneAssignmentNotFound);

        _context.DeliveryPersonZones.Remove(assignment);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
