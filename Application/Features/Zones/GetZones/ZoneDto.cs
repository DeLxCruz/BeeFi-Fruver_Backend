namespace Application.Features.Zones.GetZones;

public record ZoneDto(
    Guid Id,
    string Name,
    string City,
    string Department,
    bool IsActive,
    decimal DeliveryBaseCost,
    DateTime CreatedAt
);
