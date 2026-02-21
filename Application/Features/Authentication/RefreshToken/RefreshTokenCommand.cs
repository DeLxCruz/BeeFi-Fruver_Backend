using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Authentication.RefreshToken
{
    public record RefreshTokenCommand(
        string RefreshToken,
        string? DeviceInfo = null,
        string? IpAddress = null
    ) : IRequest<Result<RefreshTokenResponse>>;
}