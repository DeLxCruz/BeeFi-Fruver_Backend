using Domain.Abstractions;

namespace Domain.Entities;

public class LoyaltyPoints : Entity
{
    public Guid UserId { get; private set; }
    public int TotalPoints { get; private set; }
    public int AvailablePoints { get; private set; }
    public int RedeemedPoints { get; private set; }
    public int CurrentMultiplier { get; private set; }
    public DateTime LastUpdated { get; private set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<PointsTransaction> Transactions { get; private set; } = new List<PointsTransaction>();

    private LoyaltyPoints() { }

    private LoyaltyPoints(Guid id) : base(id) { }

    public static LoyaltyPoints Create(Guid userId, int initialMultiplier = 1)
    {
        return new LoyaltyPoints(Guid.NewGuid())
        {
            UserId = userId,
            TotalPoints = 0,
            AvailablePoints = 0,
            RedeemedPoints = 0,
            CurrentMultiplier = initialMultiplier,
            LastUpdated = DateTime.UtcNow
        };
    }

    public void AddPoints(int points)
    {
        TotalPoints += points;
        AvailablePoints += points;
        LastUpdated = DateTime.UtcNow;
    }

    public bool CanRedeem(int points)
    {
        return AvailablePoints >= points;
    }

    public void RedeemPoints(int points)
    {
        if (!CanRedeem(points))
            throw new InvalidOperationException($"Puntos insuficientes. Disponibles: {AvailablePoints}, Solicitados: {points}");

        AvailablePoints -= points;
        RedeemedPoints += points;
        LastUpdated = DateTime.UtcNow;
    }

    public void UpdateMultiplier(int multiplier)
    {
        CurrentMultiplier = multiplier;
        LastUpdated = DateTime.UtcNow;
    }
}