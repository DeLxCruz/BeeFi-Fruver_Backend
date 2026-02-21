using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Guid userId, string email, List<string> roles);
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateToken(string token);
    }
}