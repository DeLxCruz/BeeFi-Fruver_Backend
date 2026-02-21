using Domain.Enums;

namespace API.Contracts.Deliveries;

public record UpdateDeliveryStatusRequest(
    DeliveryStatus NewStatus,
    double? Latitude = null,
    double? Longitude = null,
    string? Notes = null);
