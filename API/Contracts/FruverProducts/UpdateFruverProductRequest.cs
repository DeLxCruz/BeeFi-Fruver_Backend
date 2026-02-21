namespace API.Contracts.FruverProducts;

public record UpdateFruverProductRequest(
    decimal Price,
    int Stock,
    decimal DiscountPercentage,
    decimal BeeFiExclusiveDiscount,
    bool IsAvailable);
