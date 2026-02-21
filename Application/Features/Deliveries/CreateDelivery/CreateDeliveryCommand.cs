using Domain.Primitives;
using MediatR;

namespace Application.Features.Deliveries.CreateDelivery;

public record CreateDeliveryCommand(
    Guid OrderId,
    DateTime? EstimatedDeliveryTime = null) : IRequest<Result<Guid>>;
