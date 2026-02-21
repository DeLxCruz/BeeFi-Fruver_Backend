namespace API.Contracts.Addresses;

public record UpdateAddressRequest(
    string AliasName,
    string Street,
    string HouseNumber,
    string Neighborhood,
    double? Latitude,
    double? Longitude,
    bool IsDefault);
