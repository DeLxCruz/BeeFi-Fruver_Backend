using Domain.Enums;

namespace API.Contracts.Orders;

public record UpdateOrderStatusRequest(
    OrderStatus NewStatus,
    string? Notes);
