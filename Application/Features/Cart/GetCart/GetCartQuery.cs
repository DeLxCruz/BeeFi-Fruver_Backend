using Domain.Primitives;
using MediatR;

namespace Application.Features.Cart.GetCart;

public record GetCartQuery : IRequest<Result<CartDto>>;
