namespace Application.Features.Loyalty.GetMyLoyaltyPoints;

public record LoyaltyPointsDto(
    Guid UserId,
    int TotalPoints,
    int AvailablePoints,
    int RedeemedPoints,
    int CurrentMultiplier,
    DateTime LastUpdated);
