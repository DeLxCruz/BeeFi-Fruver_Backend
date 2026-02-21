using Domain.Primitives;
using MediatR;

namespace Application.Features.Authentication.Login
{
    public record LoginCommand(
        string Email,
        string Password,
        string? DeviceInfo = null,
        string? IpAddress = null
    ) : IRequest<Result<LoginResponse>>;
}