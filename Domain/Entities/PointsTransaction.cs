using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class PointsTransaction : Entity
{
    public Guid UserId { get; private set; }
    public Guid? OrderId { get; private set; }
    public int Points { get; private set; }
    public PointsTransactionType Type { get; private set; }
    public string Description { get; private set; } = null!;
    public bool IsBeeFiBonus { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Order? Order { get; set; }

    private PointsTransaction() { }

    private PointsTransaction(Guid id) : base(id) { }

    public static PointsTransaction Create(
        Guid userId,
        int points,
        PointsTransactionType type,
        string description,
        bool isBeeFiBonus = false,
        Guid? orderId = null)
    {
        return new PointsTransaction(Guid.NewGuid())
        {
            UserId = userId,
            OrderId = orderId,
            Points = points,
            Type = type,
            Description = description,
            IsBeeFiBonus = isBeeFiBonus,
            CreatedAt = DateTime.UtcNow
        };
    }
}