using Application.Features.Products.GetProducts;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Products.GetProductById;

public record GetProductByIdQuery(Guid ProductId) : IRequest<Result<ProductDto>>;
