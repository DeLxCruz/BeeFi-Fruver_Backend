using Domain.Primitives;
using MediatR;

namespace Application.Features.Deliveries.RemoveDeliveryPersonFromZone;

public record RemoveDeliveryPersonFromZoneCommand(
    Guid DeliveryPersonId,
    Guid ZoneId) : IRequest<Result>;
