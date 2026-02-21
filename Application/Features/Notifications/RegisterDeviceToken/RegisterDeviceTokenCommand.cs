using Domain.Enums;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Notifications.RegisterDeviceToken;

public record RegisterDeviceTokenCommand(
    string Token,
    DevicePlatform Platform) : IRequest<Result>;
