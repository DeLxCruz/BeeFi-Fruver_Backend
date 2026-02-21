using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Authentication.Logout
{
    public record LogoutResponse(
        Guid UserId,
        int TokensRevoked,
        string Message
    );
}