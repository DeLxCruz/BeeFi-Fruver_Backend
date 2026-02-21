using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Zones.UpdateZone;

public class UpdateZoneCommandHandler
    : IRequestHandler<UpdateZoneCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateZoneCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        UpdateZoneCommand request,
        CancellationToken cancellationToken)
    {
        var zone = await _context.Zones
            .FirstOrDefaultAsync(z => z.Id == request.ZoneId, cancellationToken);

        if (zone is null)
            return Result.Failure(ZoneErrors.NotFound);

        zone.Update(request.Name, request.City, request.Department, request.DeliveryBaseCost);

        if (request.IsActive)
            zone.Activate();
        else
            zone.Deactivate();

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
