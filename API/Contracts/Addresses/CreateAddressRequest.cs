namespace API.Contracts.Addresses;

public record CreateAddressRequest(
    Guid ZoneId,
    string AliasName,
    string Street,
    string HouseNumber,
    string Neighborhood,
    double? Latitude,
    double? Longitude,
    bool IsDefault);
