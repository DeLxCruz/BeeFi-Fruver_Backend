using Domain.Enums;

namespace Application.Features.Deliveries.GetDeliveryByOrder;

public record DeliveryStatusHistoryDto(
    DeliveryStatus Status,
    DateTime Timestamp,
    double? Latitude,
    double? Longitude,
    string? Notes,
    Guid? UpdatedBy);
