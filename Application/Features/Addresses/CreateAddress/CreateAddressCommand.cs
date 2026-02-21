using Domain.Primitives;
using MediatR;

namespace Application.Features.Addresses.CreateAddress;

public record CreateAddressCommand(
    Guid ZoneId,
    string AliasName,
    string Street,
    string HouseNumber,
    string Neighborhood,
    double? Latitude,
    double? Longitude,
    bool IsDefault) : IRequest<Result<Guid>>;
