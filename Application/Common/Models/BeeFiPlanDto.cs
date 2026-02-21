namespace Application.Common.Models;

public record BeeFiPlanDto(
    int Id,
    string Name,
    decimal MonthlyPrice,
    int SpeedMbps
);
