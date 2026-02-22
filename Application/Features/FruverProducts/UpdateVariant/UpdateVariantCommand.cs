using Domain.Primitives;
using MediatR;

namespace Application.Features.FruverProducts.UpdateVariant;

public record UpdateVariantCommand(
    Guid VariantId,
    string Name,
    decimal PriceAdjustment,
    int Stock,
    bool IsActive,
    int DisplayOrder,
    string? SKU = null) : IRequest<Result>;
