using Domain.Primitives;
using MediatR;

namespace Application.Features.Deliveries.AssignDeliveryPerson;

public record AssignDeliveryPersonCommand(
    Guid DeliveryId,
    Guid DeliveryPersonId) : IRequest<Result>;
