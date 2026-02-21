using Domain.Primitives;
using MediatR;

namespace Application.Features.Deliveries.GetDeliveryByOrder;

public record GetDeliveryByOrderQuery(Guid OrderId) : IRequest<Result<DeliveryDto>>;
