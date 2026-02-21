using Application.Features.FruverProducts.GetFruverProducts;
using Domain.Primitives;
using MediatR;

namespace Application.Features.FruverProducts.GetFruverProductById;

public record GetFruverProductByIdQuery(Guid FruverProductId) : IRequest<Result<FruverProductDto>>;
