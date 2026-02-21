using Domain.Enums;

namespace API.Contracts.Payments;

public record InitiatePaymentRequest(
    Guid OrderId,
    PaymentMethod PaymentMethod,
    string ReturnUrl);
