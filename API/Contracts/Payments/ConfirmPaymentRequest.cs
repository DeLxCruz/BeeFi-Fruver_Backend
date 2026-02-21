namespace API.Contracts.Payments;

public record ConfirmPaymentRequest(
    Guid OrderId,
    string TransactionId,
    string? GatewayResponse = null);
