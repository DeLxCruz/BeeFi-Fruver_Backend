namespace API.Contracts.FruverProducts;

public record UpdateVariantRequest(
    string Name,
    decimal PriceAdjustment,
    int Stock,
    bool IsActive,
    int DisplayOrder,
    string? SKU = null);
