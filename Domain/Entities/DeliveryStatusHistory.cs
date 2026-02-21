using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class DeliveryStatusHistory : Entity
{
    public Guid DeliveryId { get; private set; }
    public DeliveryStatus Status { get; private set; }
    public DateTime Timestamp { get; private set; }
    public decimal? Latitude { get; private set; }
    public decimal? Longitude { get; private set; }
    public string? Notes { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    // Navigation properties
    public virtual Delivery Delivery { get; set; } = null!;
    public virtual User? UpdatedByUser { get; set; }

    private DeliveryStatusHistory() { }

    private DeliveryStatusHistory(Guid id) : base(id) { }

    public static DeliveryStatusHistory Create(
        Guid deliveryId,
        DeliveryStatus status,
        Guid? updatedBy = null,
        string? notes = null,
        decimal? latitude = null,
        decimal? longitude = null)
    {
        return new DeliveryStatusHistory(Guid.NewGuid())
        {
            DeliveryId = deliveryId,
            Status = status,
            Timestamp = DateTime.UtcNow,
            Latitude = latitude,
            Longitude = longitude,
            Notes = notes,
            UpdatedBy = updatedBy
        };
    }
}