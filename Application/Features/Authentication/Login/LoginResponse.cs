using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Authentication.Login
{
    public record LoginResponse(
        string AccessToken,
        string RefreshToken,
        DateTime ExpiresAt,
        Guid UserId,
        string Email,
        string FirstName,
        string LastName,
        List<string> Roles,
        bool HasBeeFiSubscription,
        string? BeeFiPlanName,
        decimal BeeFiDiscountPercentage,
        int BeeFiPointsMultiplier
    );
}