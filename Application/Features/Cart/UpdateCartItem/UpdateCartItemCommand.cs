using Domain.Primitives;
using MediatR;

namespace Application.Features.Cart.UpdateCartItem;

public record UpdateCartItemCommand(
    Guid CartItemId,
    int Quantity) : IRequest<Result>;
