using Domain.Enums;

namespace Application.Common.Models;

public class PaymentStatusResult
{
    public bool IsSuccess { get; init; }
    public PaymentStatus Status { get; init; }
    public string? TransactionId { get; init; }
    public string? ErrorMessage { get; init; }
}
