using Domain.Primitives;
using MediatR;

namespace Application.Features.FruverProducts.UpdateFruverProduct;

public record UpdateFruverProductCommand(
    Guid FruverProductId,
    decimal Price,
    int Stock,
    decimal DiscountPercentage,
    decimal BeeFiExclusiveDiscount,
    bool IsAvailable) : IRequest<Result>;
