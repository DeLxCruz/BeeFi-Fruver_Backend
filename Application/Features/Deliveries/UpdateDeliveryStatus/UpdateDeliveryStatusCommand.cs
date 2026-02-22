using Domain.Enums;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Deliveries.UpdateDeliveryStatus;

public record UpdateDeliveryStatusCommand(
    Guid DeliveryId,
    DeliveryStatus NewStatus,
    double? Latitude = null,
    double? Longitude = null,
    string? Notes = null,
    string? DeliveryProofUrl = null,
    string? DeliveryPin = null) : IRequest<Result>;
