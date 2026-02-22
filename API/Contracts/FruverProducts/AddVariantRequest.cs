namespace API.Contracts.FruverProducts;

public record AddVariantRequest(
    string Name,
    decimal PriceAdjustment,
    int Stock,
    int DisplayOrder,
    string? SKU = null);
