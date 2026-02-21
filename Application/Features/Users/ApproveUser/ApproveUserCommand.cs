using Domain.Primitives;
using MediatR;

namespace Application.Features.Users.ApproveUser;

/// <summary>
/// Comando para aprobar una cuenta de usuario pendiente
/// </summary>
public record ApproveUserCommand(Guid UserId) : IRequest<Result>;
