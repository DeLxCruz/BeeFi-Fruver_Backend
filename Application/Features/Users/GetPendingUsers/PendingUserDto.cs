using Domain.Enums;

namespace Application.Features.Users.GetPendingUsers;

/// <summary>
/// DTO para usuario pendiente de aprobación
/// </summary>
public record PendingUserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string? ProfileImageUrl,
    AccountStatus AccountStatus,
    DateTime CreatedAt,
    int DaysPending);
