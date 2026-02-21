using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Zones.GetZoneFruvers;

public class GetZoneFruversQueryHandler
    : IRequestHandler<GetZoneFruversQuery, Result<List<ZoneFruverDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetZoneFruversQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ZoneFruverDto>>> Handle(
        GetZoneFruversQuery request,
        CancellationToken cancellationToken)
    {
        var zoneExists = await _context.Zones
            .AnyAsync(z => z.Id == request.ZoneId, cancellationToken);

        if (!zoneExists)
            return Result.Failure<List<ZoneFruverDto>>(ZoneErrors.NotFound);

        var fruvers = await _context.FruverZones
            .Where(fz => fz.ZoneId == request.ZoneId)
            .Select(fz => new ZoneFruverDto(
                fz.FruverId,
                fz.Fruver.FirstName + " " + fz.Fruver.LastName,
                fz.Fruver.Email,
                fz.AssignedAt))
            .ToListAsync(cancellationToken);

        return Result.Success(fruvers);
    }
}
