using Domain.Primitives;
using MediatR;

namespace Application.Features.Zones.RemoveFruverFromZone;

public record RemoveFruverFromZoneCommand(
    Guid FruverId,
    Guid ZoneId
) : IRequest<Result>;
