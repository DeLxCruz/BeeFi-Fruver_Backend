using Domain.Primitives;
using MediatR;

namespace Application.Features.Deliveries.AssignDeliveryPersonToZone;

public record AssignDeliveryPersonToZoneCommand(
    Guid DeliveryPersonId,
    Guid ZoneId) : IRequest<Result>;
