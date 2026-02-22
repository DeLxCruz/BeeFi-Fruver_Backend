using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class Delivery : Entity
{
    public Guid OrderId { get; private set; }
    public Guid? DeliveryPersonId { get; private set; }
    public DeliveryStatus Status { get; private set; }
    public DateTime? EstimatedDeliveryTime { get; private set; }
    public DateTime? ActualDeliveryTime { get; private set; }
    public string? TrackingNotes { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Modalidad dual de transporte
    public DeliveryMode DeliveryMode { get; private set; }
    public decimal? SellerDeliveryFee { get; private set; }
    public string? DeliveryProofUrl { get; private set; }
    public string? DeliveryPin { get; private set; }
    public string? SellerDeliveryPersonName { get; private set; }

    // Navigation properties
    public virtual Order Order { get; set; } = null!;
    public virtual User? DeliveryPerson { get; set; }
    public virtual ICollection<DeliveryStatusHistory> StatusHistory { get; private set; } = new List<DeliveryStatusHistory>();

    private Delivery() { }

    private Delivery(Guid id) : base(id) { }

    public static Delivery Create(
        Guid orderId,
        DeliveryMode deliveryMode = DeliveryMode.BeeFiLogistics,
        DateTime? estimatedDeliveryTime = null,
        decimal? sellerDeliveryFee = null,
        string? sellerDeliveryPersonName = null)
    {
        return new Delivery(Guid.NewGuid())
        {
            OrderId = orderId,
            Status = DeliveryStatus.Pending,
            EstimatedDeliveryTime = estimatedDeliveryTime,
            DeliveryMode = deliveryMode,
            SellerDeliveryFee = sellerDeliveryFee,
            SellerDeliveryPersonName = sellerDeliveryPersonName,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void AssignDeliveryPerson(Guid deliveryPersonId)
    {
        DeliveryPersonId = deliveryPersonId;
        Status = DeliveryStatus.Assigned;
    }

    public void MarkAsPickedUp()
    {
        Status = DeliveryStatus.PickedUp;
    }

    public void StartRoute()
    {
        Status = DeliveryStatus.OnRoute;
    }

    public void MarkAsNearDestination()
    {
        Status = DeliveryStatus.NearDestination;
    }

    public void Complete()
    {
        Status = DeliveryStatus.Delivered;
        ActualDeliveryTime = DateTime.UtcNow;
    }

    public void MarkAsFailed(string notes)
    {
        Status = DeliveryStatus.Failed;
        TrackingNotes = notes;
    }

    public void UpdateEstimatedTime(DateTime estimatedTime)
    {
        EstimatedDeliveryTime = estimatedTime;
    }

    public void UpdateStatus(DeliveryStatus newStatus, string? trackingNotes = null, string? proofUrl = null, string? pin = null)
    {
        Status = newStatus;
        if (trackingNotes is not null) TrackingNotes = trackingNotes;
        if (proofUrl is not null) DeliveryProofUrl = proofUrl;
        if (pin is not null) DeliveryPin = pin;
        if (newStatus == DeliveryStatus.Delivered) ActualDeliveryTime = DateTime.UtcNow;
    }
}