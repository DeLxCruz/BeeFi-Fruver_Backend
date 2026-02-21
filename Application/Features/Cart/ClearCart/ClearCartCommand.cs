using Domain.Primitives;
using MediatR;

namespace Application.Features.Cart.ClearCart;

public record ClearCartCommand : IRequest<Result>;
