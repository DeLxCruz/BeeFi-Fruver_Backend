namespace Application.Features.Addresses.Common;

public record AddressDto(
    Guid Id,
    Guid ZoneId,
    string ZoneName,
    string City,
    string AliasName,
    string Street,
    string HouseNumber,
    string Neighborhood,
    double? Latitude,
    double? Longitude,
    bool IsDefault,
    DateTime CreatedAt);
