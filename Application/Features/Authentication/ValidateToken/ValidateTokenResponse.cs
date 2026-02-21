using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Authentication.ValidateToken
{
    public record ValidateTokenResponse(
        Guid UserId,
        string Email,
        string FirstName,
        string LastName,
        List<string> Roles,
        bool IsValid
    );
}