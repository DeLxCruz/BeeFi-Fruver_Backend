using Domain.Enums;

namespace API.Contracts.Orders;

public record CreateOrderRequest(
    Guid AddressId,
    PaymentMethod PaymentMethod,
    string? Notes);
