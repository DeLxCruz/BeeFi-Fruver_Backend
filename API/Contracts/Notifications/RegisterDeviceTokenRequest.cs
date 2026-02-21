using Domain.Enums;

namespace API.Contracts.Notifications;

public record RegisterDeviceTokenRequest(
    string Token,
    DevicePlatform Platform);
