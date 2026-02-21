using Domain.Primitives;
using MediatR;

namespace Application.Features.Zones.UpdateZone;

public record UpdateZoneCommand(
    Guid ZoneId,
    string Name,
    string City,
    string Department,
    decimal DeliveryBaseCost,
    bool IsActive
) : IRequest<Result>;
