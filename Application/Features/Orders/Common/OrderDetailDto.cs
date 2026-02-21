using Domain.Enums;

namespace Application.Features.Orders.Common;

public record OrderDetailDto(
    Guid Id,
    string OrderNumber,
    OrderStatus Status,
    string StatusName,
    Guid AddressId,
    string AddressDetail,
    decimal Subtotal,
    decimal DeliveryFee,
    decimal Discount,
    decimal BeeFiDiscount,
    decimal Total,
    PaymentMethod PaymentMethod,
    PaymentStatus PaymentStatus,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<OrderItemDto> Items);
