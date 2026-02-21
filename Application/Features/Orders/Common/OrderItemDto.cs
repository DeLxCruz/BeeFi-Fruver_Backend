using Domain.Enums;

namespace Application.Features.Orders.Common;

public record OrderItemDto(
    Guid Id,
    Guid FruverProductId,
    string ProductName,
    string? ProductImageUrl,
    Guid FruverId,
    string FruverName,
    decimal UnitPrice,
    int Quantity,
    decimal Subtotal);
