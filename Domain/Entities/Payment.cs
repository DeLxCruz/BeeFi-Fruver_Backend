using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class Payment : Entity
{
    public Guid OrderId { get; private set; }
    public PaymentMethod Method { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? TransactionId { get; private set; }
    public string? GatewayResponse { get; private set; }
    public DateTime? PaymentDate { get; private set; }
    public decimal? RefundAmount { get; private set; }
    public DateTime? RefundedAt { get; private set; }
    public string? RefundReason { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public virtual Order Order { get; set; } = null!;

    private Payment() { }

    private Payment(Guid id) : base(id) { }

    public static Payment Create(
        Guid orderId,
        PaymentMethod method,
        decimal amount)
    {
        return new Payment(Guid.NewGuid())
        {
            OrderId = orderId,
            Method = method,
            Amount = amount,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsProcessing(string transactionId)
    {
        Status = PaymentStatus.Processing;
        TransactionId = transactionId;
    }

    public void Complete(string? gatewayResponse = null)
    {
        Status = PaymentStatus.Completed;
        PaymentDate = DateTime.UtcNow;
        GatewayResponse = gatewayResponse;
    }

    public void Fail(string? gatewayResponse = null)
    {
        Status = PaymentStatus.Failed;
        GatewayResponse = gatewayResponse;
    }

    public void Refund(decimal amount, string reason)
    {
        if (Status != PaymentStatus.Completed)
            throw new InvalidOperationException("Solo se pueden reembolsar pagos completados");

        if (amount <= 0 || amount > Amount)
            throw new InvalidOperationException($"Monto de reembolso inválido. Máximo permitido: {Amount}");

        Status = PaymentStatus.Refunded;
        RefundAmount = amount;
        RefundedAt = DateTime.UtcNow;
        RefundReason = reason;
    }
}