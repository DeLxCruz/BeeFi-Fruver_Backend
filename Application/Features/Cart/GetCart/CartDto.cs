namespace Application.Features.Cart.GetCart;

public record CartDto(
    Guid UserId,
    List<CartItemDto> Items,
    int TotalItems,
    decimal Subtotal,
    bool HasMixedFruvers,
    DateTime? UpdatedAt);
