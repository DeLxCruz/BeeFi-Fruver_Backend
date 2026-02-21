namespace API.Contracts.FruverProducts;

public record PublishFruverProductRequest(
    Guid ProductId,
    decimal Price,
    int Stock,
    decimal DiscountPercentage,
    decimal BeeFiExclusiveDiscount);
