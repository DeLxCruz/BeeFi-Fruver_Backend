using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Zones.AssignFruverToZone;

public class AssignFruverToZoneCommandHandler
    : IRequestHandler<AssignFruverToZoneCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public AssignFruverToZoneCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        AssignFruverToZoneCommand request,
        CancellationToken cancellationToken)
    {
        // Verificar que el usuario existe y tiene rol FruverAliado
        var fruverExists = await _context.Users
            .AnyAsync(
                u => u.Id == request.FruverId &&
                     u.UserRoles.Any(ur => ur.Role.Name == "FruverAliado"),
                cancellationToken);

        if (!fruverExists)
            return Result.Failure(ZoneErrors.FruverNotFound);

        // Verificar que la zona existe
        var zoneExists = await _context.Zones
            .AnyAsync(z => z.Id == request.ZoneId, cancellationToken);

        if (!zoneExists)
            return Result.Failure(ZoneErrors.NotFound);

        // Verificar que no esté ya asignado
        var alreadyAssigned = await _context.FruverZones
            .AnyAsync(
                fz => fz.FruverId == request.FruverId && fz.ZoneId == request.ZoneId,
                cancellationToken);

        if (alreadyAssigned)
            return Result.Failure(ZoneErrors.FruverAlreadyAssigned);

        var fruverZone = FruverZone.Create(request.FruverId, request.ZoneId);
        _context.FruverZones.Add(fruverZone);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
