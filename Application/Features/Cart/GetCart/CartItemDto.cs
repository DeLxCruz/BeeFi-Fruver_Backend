namespace Application.Features.Cart.GetCart;

public record CartItemDto(
    Guid Id,
    Guid FruverProductId,
    string ProductName,
    string? ProductImageUrl,
    Guid FruverId,
    string FruverName,
    decimal Price,
    decimal? DiscountPercentage,
    decimal? BeeFiExclusiveDiscount,
    decimal FinalPrice,
    int Quantity,
    decimal Subtotal,
    int Stock,
    bool IsAvailable,
    DateTime AddedAt);
