using Domain.Abstractions;

namespace Domain.Entities;

public class Address : Entity
{
    public Guid UserId { get; private set; }
    public Guid ZoneId { get; private set; }
    public string Label { get; private set; } = null!; // Casa, Trabajo, Otro
    public string Street { get; private set; } = null!;
    public string HouseNumber { get; private set; } = null!;
    public string AdditionalInfo { get; private set; } = null!;
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    public bool IsDefault { get; private set; }
    public bool IsDeleted { get; private set; } = false;
    public DateTime? DeletedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Zone Zone { get; set; } = null!;
    public virtual ICollection<Order> Orders { get; private set; } = new List<Order>();

    private Address() { }

    private Address(Guid id) : base(id) { }

    public static Address Create(
        Guid userId,
        Guid zoneId,
        string label,
        string street,
        string houseNumber,
        string additionalInfo,
        double? latitude = null,
        double? longitude = null,
        bool isDefault = false)
    {
        return new Address(Guid.NewGuid())
        {
            UserId = userId,
            ZoneId = zoneId,
            Label = label,
            Street = street,
            HouseNumber = houseNumber,
            AdditionalInfo = additionalInfo,
            Latitude = latitude,
            Longitude = longitude,
            IsDefault = isDefault,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string label,
        string street,
        string houseNumber,
        string additionalInfo,
        Guid zoneId)
    {
        Label = label;
        Street = street;
        HouseNumber = houseNumber;
        AdditionalInfo = additionalInfo;
        ZoneId = zoneId;
    }

    public void SetAsDefault()
    {
        IsDefault = true;
    }

    public void RemoveDefault()
    {
        IsDefault = false;
    }

    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    public void UpdateCoordinates(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
}