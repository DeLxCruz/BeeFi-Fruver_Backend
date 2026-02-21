using Application.Common.Interfaces;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Zones.GetZones;

public class GetZonesQueryHandler
    : IRequestHandler<GetZonesQuery, Result<List<ZoneDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetZonesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ZoneDto>>> Handle(
        GetZonesQuery request,
        CancellationToken cancellationToken)
    {
        var zones = await _context.Zones
            .Where(z => z.IsActive)
            .OrderBy(z => z.Name)
            .Select(z => new ZoneDto(
                z.Id,
                z.Name,
                z.City,
                z.Department,
                z.IsActive,
                z.DeliveryBaseCost,
                z.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result.Success(zones);
    }
}
