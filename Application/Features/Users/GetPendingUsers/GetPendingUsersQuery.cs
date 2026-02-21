using Domain.Primitives;
using MediatR;

namespace Application.Features.Users.GetPendingUsers;

/// <summary>
/// Query para obtener usuarios pendientes de aprobación
/// </summary>
public record GetPendingUsersQuery : IRequest<Result<List<PendingUserDto>>>;
