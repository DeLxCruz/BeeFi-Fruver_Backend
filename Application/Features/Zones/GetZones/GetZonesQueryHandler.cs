using Application.Common.Interfaces;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Zones.GetZones;

public class GetZonesQueryHandler
    : IRequestHandler<GetZonesQuery, Result<List<ZoneDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;

    public const string CacheKey = "zones:active";

    public GetZonesQueryHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Result<List<ZoneDto>>> Handle(
        GetZonesQuery request,
        CancellationToken cancellationToken)
    {
        var zones = await _cache.GetOrCreateAsync(
            CacheKey,
            async () => await _context.Zones
                .AsNoTracking()
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
                .ToListAsync(cancellationToken),
            TimeSpan.FromMinutes(30),
            cancellationToken);

        return Result.Success(zones);
    }
}
