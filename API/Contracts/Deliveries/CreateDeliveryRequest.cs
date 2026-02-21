namespace API.Contracts.Deliveries;

public record CreateDeliveryRequest(
    Guid OrderId,
    DateTime? EstimatedDeliveryTime = null);
