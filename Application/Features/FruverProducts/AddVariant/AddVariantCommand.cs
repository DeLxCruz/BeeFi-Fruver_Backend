using Domain.Primitives;
using MediatR;

namespace Application.Features.FruverProducts.AddVariant;

public record AddVariantCommand(
    Guid FruverProductId,
    string Name,
    decimal PriceAdjustment,
    int Stock,
    int DisplayOrder,
    string? SKU = null) : IRequest<Result<Guid>>;
