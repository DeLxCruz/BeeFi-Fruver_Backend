using Domain.Enums;

namespace Application.Features.Loyalty.GetMyPointsHistory;

public record PointsTransactionDto(
    Guid Id,
    PointsTransactionType Type,
    int Points,
    Guid? OrderId,
    string Description,
    bool UsedBonus,
    DateTime CreatedAt);
