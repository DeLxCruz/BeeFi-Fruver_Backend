namespace API.Contracts.Zones;

public record CreateZoneRequest(
    string Name,
    string City,
    string Department,
    decimal DeliveryBaseCost);
