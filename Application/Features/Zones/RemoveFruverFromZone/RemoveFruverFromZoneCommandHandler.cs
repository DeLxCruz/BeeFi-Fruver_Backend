using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Zones.RemoveFruverFromZone;

public class RemoveFruverFromZoneCommandHandler
    : IRequestHandler<RemoveFruverFromZoneCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public RemoveFruverFromZoneCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        RemoveFruverFromZoneCommand request,
        CancellationToken cancellationToken)
    {
        var fruverZone = await _context.FruverZones
            .FirstOrDefaultAsync(
                fz => fz.FruverId == request.FruverId && fz.ZoneId == request.ZoneId,
                cancellationToken);

        if (fruverZone is null)
            return Result.Failure(ZoneErrors.FruverNotAssigned);

        _context.FruverZones.Remove(fruverZone);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
