using Domain.Enums;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Deliveries.CreateDelivery;

public record CreateDeliveryCommand(
    Guid OrderId,
    DeliveryMode DeliveryMode = DeliveryMode.BeeFiLogistics,
    DateTime? EstimatedDeliveryTime = null,
    decimal? SellerDeliveryFee = null,
    string? SellerDeliveryPersonName = null) : IRequest<Result<Guid>>;
