namespace API.Contracts.Zones;

public record UpdateZoneRequest(
    string Name,
    string City,
    string Department,
    decimal DeliveryBaseCost,
    bool IsActive);
