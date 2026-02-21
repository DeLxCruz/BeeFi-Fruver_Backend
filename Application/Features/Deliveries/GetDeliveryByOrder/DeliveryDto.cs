using Domain.Enums;

namespace Application.Features.Deliveries.GetDeliveryByOrder;

public record DeliveryDto(
    Guid Id,
    Guid OrderId,
    string OrderNumber,
    Guid? DeliveryPersonId,
    string? DeliveryPersonName,
    DeliveryStatus Status,
    DateTime? EstimatedDeliveryTime,
    DateTime? ActualDeliveryTime,
    string? TrackingNotes,
    List<DeliveryStatusHistoryDto> StatusHistory);
