namespace API.Contracts.Cart;

public record AddToCartRequest(
    Guid FruverProductId,
    int Quantity);
