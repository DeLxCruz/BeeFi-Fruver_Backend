namespace Domain.Entities;

public class FruverZone
{
    public Guid FruverId { get; set; }
    public Guid ZoneId { get; set; }
    public DateTime AssignedAt { get; set; }

    // Navigation properties
    public virtual User Fruver { get; set; } = null!;
    public virtual Zone Zone { get; set; } = null!;

    private FruverZone() { }

    public static FruverZone Create(Guid fruverId, Guid zoneId)
    {
        return new FruverZone
        {
            FruverId = fruverId,
            ZoneId = zoneId,
            AssignedAt = DateTime.UtcNow
        };
    }
}
