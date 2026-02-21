using Domain.Primitives;
using MediatR;

namespace Application.Features.Zones.CreateZone;

public record CreateZoneCommand(
    string Name,
    string City,
    string Department,
    decimal DeliveryBaseCost
) : IRequest<Result<CreateZoneResponse>>;
