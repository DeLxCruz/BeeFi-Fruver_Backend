using Domain.Enums;

namespace Application.Features.Orders.Common;

public record OrderSummaryDto(
    Guid Id,
    string OrderNumber,
    OrderStatus Status,
    string StatusName,
    decimal Subtotal,
    decimal DeliveryFee,
    decimal Discount,
    decimal BeeFiDiscount,
    decimal Total,
    PaymentMethod PaymentMethod,
    PaymentStatus PaymentStatus,
    DateTime CreatedAt,
    int ItemCount,
    List<string> FruverNames);
