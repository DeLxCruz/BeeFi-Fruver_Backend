using Domain.Enums;

namespace Application.Features.Payments.GetPaymentByOrder;

public record PaymentDto(
    Guid Id,
    Guid OrderId,
    string OrderNumber,
    PaymentMethod Method,
    PaymentStatus Status,
    decimal Amount,
    string? TransactionId,
    string? GatewayResponse,
    decimal? RefundAmount,
    DateTime? RefundedAt,
    string? RefundReason,
    DateTime CreatedAt);
