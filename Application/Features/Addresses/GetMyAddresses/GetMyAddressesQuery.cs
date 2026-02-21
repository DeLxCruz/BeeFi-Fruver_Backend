using Application.Features.Addresses.Common;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Addresses.GetMyAddresses;

public record GetMyAddressesQuery : IRequest<Result<List<AddressDto>>>;
