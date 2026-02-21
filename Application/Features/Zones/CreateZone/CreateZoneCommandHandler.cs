using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Zones.CreateZone;

public class CreateZoneCommandHandler
    : IRequestHandler<CreateZoneCommand, Result<CreateZoneResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;

    public CreateZoneCommandHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Result<CreateZoneResponse>> Handle(
        CreateZoneCommand request,
        CancellationToken cancellationToken)
    {
        var exists = await _context.Zones
            .AnyAsync(
                z => z.Name == request.Name && z.City == request.City,
                cancellationToken);

        if (exists)
            return Result.Failure<CreateZoneResponse>(ZoneErrors.AlreadyExists);

        var zone = Zone.Create(
            request.Name,
            request.City,
            request.Department,
            request.DeliveryBaseCost);

        _context.Zones.Add(zone);
        await _context.SaveChangesAsync(cancellationToken);
        await _cache.RemoveAsync("zones:active", cancellationToken);

        return Result.Success(new CreateZoneResponse(
            zone.Id,
            zone.Name,
            zone.City,
            zone.Department,
            zone.DeliveryBaseCost));
    }
}
