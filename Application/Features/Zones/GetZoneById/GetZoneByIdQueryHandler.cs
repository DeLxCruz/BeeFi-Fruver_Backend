using Application.Common.Interfaces;
using Application.Features.Zones.GetZones;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Zones.GetZoneById;

public class GetZoneByIdQueryHandler
    : IRequestHandler<GetZoneByIdQuery, Result<ZoneDto>>
{
    private readonly IApplicationDbContext _context;

    public GetZoneByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ZoneDto>> Handle(
        GetZoneByIdQuery request,
        CancellationToken cancellationToken)
    {
        var zone = await _context.Zones
            .Where(z => z.Id == request.ZoneId)
            .Select(z => new ZoneDto(
                z.Id,
                z.Name,
                z.City,
                z.Department,
                z.IsActive,
                z.DeliveryBaseCost,
                z.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        if (zone is null)
            return Result.Failure<ZoneDto>(ZoneErrors.NotFound);

        return Result.Success(zone);
    }
}
