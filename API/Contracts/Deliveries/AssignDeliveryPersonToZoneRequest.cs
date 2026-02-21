namespace API.Contracts.Deliveries;

public record AssignDeliveryPersonToZoneRequest(
    Guid DeliveryPersonId,
    Guid ZoneId);
