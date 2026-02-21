using Domain.Primitives;
using MediatR;

namespace Application.Features.Cart.RemoveFromCart;

public record RemoveFromCartCommand(Guid CartItemId) : IRequest<Result>;
