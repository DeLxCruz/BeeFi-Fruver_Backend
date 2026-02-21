using Domain.Primitives;
using MediatR;

namespace Application.Features.Cart.AddToCart;

public record AddToCartCommand(
    Guid FruverProductId,
    int Quantity) : IRequest<Result>;
