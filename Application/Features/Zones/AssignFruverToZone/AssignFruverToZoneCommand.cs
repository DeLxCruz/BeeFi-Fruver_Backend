using Domain.Primitives;
using MediatR;

namespace Application.Features.Zones.AssignFruverToZone;

public record AssignFruverToZoneCommand(
    Guid FruverId,
    Guid ZoneId
) : IRequest<Result>;
