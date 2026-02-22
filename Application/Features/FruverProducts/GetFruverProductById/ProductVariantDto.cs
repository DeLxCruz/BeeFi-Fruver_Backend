namespace Application.Features.FruverProducts.GetFruverProductById;

public record ProductVariantDto(
    Guid Id,
    string Name,
    string? SKU,
    decimal PriceAdjustment,
    int Stock,
    bool IsActive,
    int DisplayOrder);
