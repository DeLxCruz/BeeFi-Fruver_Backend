namespace API.Contracts.Payments;

public record ProcessRefundRequest(
    Guid OrderId,
    decimal Amount,
    string Reason);
