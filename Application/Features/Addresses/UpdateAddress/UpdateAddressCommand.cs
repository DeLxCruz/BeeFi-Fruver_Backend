using Domain.Primitives;
using MediatR;

namespace Application.Features.Addresses.UpdateAddress;

public record UpdateAddressCommand(
    Guid AddressId,
    string AliasName,
    string Street,
    string HouseNumber,
    string Neighborhood,
    double? Latitude,
    double? Longitude,
    bool IsDefault) : IRequest<Result>;
