using Domain.Abstractions;

namespace Domain.Entities;

public class DeliveryPersonZone : Entity
{
    public Guid DeliveryPersonId { get; private set; }
    public Guid ZoneId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime AssignedAt { get; private set; }

    // Navigation properties
    public virtual User DeliveryPerson { get; private set; } = null!;
    public virtual Zone Zone { get; private set; } = null!;

    private DeliveryPersonZone() { }

    private DeliveryPersonZone(Guid id) : base(id) { }

    public static DeliveryPersonZone Create(Guid deliveryPersonId, Guid zoneId)
    {
        return new DeliveryPersonZone(Guid.NewGuid())
        {
            DeliveryPersonId = deliveryPersonId,
            ZoneId = zoneId,
            IsActive = true,
            AssignedAt = DateTime.UtcNow
        };
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
