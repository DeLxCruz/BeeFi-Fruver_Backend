using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Authentication.Logout
{
    public record LogoutCommand(
        Guid UserId,
        string? RefreshToken = null,
        bool RevokeAllTokens = false
    ) : IRequest<Result<LogoutResponse>>;
}