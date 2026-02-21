namespace Application.Features.Zones.CreateZone;

public record CreateZoneResponse(
    Guid Id,
    string Name,
    string City,
    string Department,
    decimal DeliveryBaseCost
);
