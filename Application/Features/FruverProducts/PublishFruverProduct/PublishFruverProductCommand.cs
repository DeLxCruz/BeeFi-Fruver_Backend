using Domain.Primitives;
using MediatR;

namespace Application.Features.FruverProducts.PublishFruverProduct;

public record PublishFruverProductCommand(
    Guid ProductId,
    decimal Price,
    int Stock,
    decimal DiscountPercentage,
    decimal BeeFiExclusiveDiscount) : IRequest<Result<PublishFruverProductResponse>>;
