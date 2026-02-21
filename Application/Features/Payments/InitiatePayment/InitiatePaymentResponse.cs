namespace Application.Features.Payments.InitiatePayment;

public record InitiatePaymentResponse(
    Guid PaymentId,
    bool IsCashOnDelivery,
    string? RedirectUrl,
    string? TransactionId);
