using Domain.Enums;

namespace Application.Common.Models;

public class PaymentInitRequest
{
    public Guid OrderId { get; init; }
    public string OrderNumber { get; init; } = null!;
    public decimal Amount { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
    public string UserEmail { get; init; } = null!;
    public string UserName { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string ReturnUrl { get; init; } = null!;
}
