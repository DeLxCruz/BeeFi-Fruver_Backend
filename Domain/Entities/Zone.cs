using Domain.Abstractions;

namespace Domain.Entities;

public class Zone : Entity
{
    public string Name { get; private set; } = null!;
    public string City { get; private set; } = null!;
    public string Department { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public decimal DeliveryBaseCost { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public virtual ICollection<Address> Addresses { get; private set; } = new List<Address>();
    public virtual ICollection<FruverZone> FruverZones { get; private set; } = new List<FruverZone>();

    private Zone() { }

    private Zone(Guid id) : base(id) { }

    public static Zone Create(
        string name,
        string city,
        string department,
        decimal deliveryBaseCost)
    {
        return new Zone(Guid.NewGuid())
        {
            Name = name,
            City = city,
            Department = department,
            DeliveryBaseCost = deliveryBaseCost,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string city, string department, decimal deliveryBaseCost)
    {
        Name = name;
        City = city;
        Department = department;
        DeliveryBaseCost = deliveryBaseCost;
    }

    public void UpdateDeliveryCost(decimal cost)
    {
        DeliveryBaseCost = cost;
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