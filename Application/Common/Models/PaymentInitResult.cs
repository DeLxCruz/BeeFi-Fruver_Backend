namespace Application.Common.Models;

public class PaymentInitResult
{
    public bool IsSuccess { get; init; }
    public string? RedirectUrl { get; init; }
    public string? TransactionId { get; init; }
    public string? ErrorMessage { get; init; }
}
