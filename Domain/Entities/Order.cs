using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class Order : Entity, IAuditableEntity
{
    public string OrderNumber { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public Guid AddressId { get; private set; }
    public OrderStatus Status { get; private set; }

    public decimal Subtotal { get; private set; }
    public decimal DeliveryFee { get; private set; }
    public decimal Discount { get; private set; }
    public decimal BeeFiDiscount { get; private set; }
    public decimal Total { get; private set; }

    public PaymentMethod PaymentMethod { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }

    public string? Notes { get; private set; }

    // Comisión
    public decimal CommissionAmount { get; private set; }
    public Guid? CommissionRuleId { get; private set; }
    public string? CommissionRuleName { get; private set; }

    // Modalidad de transporte
    public DeliveryMode DeliveryMode { get; private set; }

    // IAuditableEntity
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Address Address { get; set; } = null!;
    public virtual ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();
    public virtual Payment? Payment { get; private set; }
    public virtual Delivery? Delivery { get; private set; }
    public virtual ICollection<PointsTransaction> PointsTransactions { get; private set; } = new List<PointsTransaction>();
    public virtual ICollection<BeeFiBenefitUsage> BeeFiBenefitUsages { get; private set; } = new List<BeeFiBenefitUsage>();

    private Order() { }

    private Order(Guid id) : base(id) { }

    public static Order Create(
        string orderNumber,
        Guid userId,
        Guid addressId,
        decimal subtotal,
        decimal deliveryFee,
        decimal discount,
        decimal beeFiDiscount,
        PaymentMethod paymentMethod,
        string? notes = null)
    {
        var total = subtotal - discount - beeFiDiscount + deliveryFee;

        return new Order(Guid.NewGuid())
        {
            OrderNumber = orderNumber,
            UserId = userId,
            AddressId = addressId,
            Status = OrderStatus.Pending,
            Subtotal = subtotal,
            DeliveryFee = deliveryFee,
            Discount = discount,
            BeeFiDiscount = beeFiDiscount,
            Total = total,
            PaymentMethod = paymentMethod,
            PaymentStatus = PaymentStatus.Pending,
            Notes = notes,
            CommissionAmount = 0m,
            DeliveryMode = DeliveryMode.BeeFiLogistics,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void SetCommission(decimal amount, Guid? ruleId, string? ruleName)
    {
        CommissionAmount = amount;
        CommissionRuleId = ruleId;
        CommissionRuleName = ruleName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePaymentStatus(PaymentStatus status)
    {
        PaymentStatus = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Confirm()
    {
        Status = OrderStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void StartPreparing()
    {
        Status = OrderStatus.Preparing;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkReadyForPickup()
    {
        Status = OrderStatus.ReadyForPickup;
        UpdatedAt = DateTime.UtcNow;
    }

    public void StartDelivery()
    {
        Status = OrderStatus.InDelivery;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        Status = OrderStatus.Delivered;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string? reason = null)
    {
        if (Status == OrderStatus.Delivered)
            throw new InvalidOperationException("No se puede cancelar un pedido ya entregado");

        Status = OrderStatus.Cancelled;
        if (reason is not null) Notes = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(OrderStatus newStatus, string? notes = null)
    {
        Status = newStatus;
        if (notes is not null) Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool CanBeCancelled => Status is OrderStatus.Pending or OrderStatus.Confirmed;
}