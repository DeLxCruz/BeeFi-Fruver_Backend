using Domain.Primitives;
using MediatR;

namespace Application.Features.Addresses.DeleteAddress;

public record DeleteAddressCommand(Guid AddressId) : IRequest<Result>;
