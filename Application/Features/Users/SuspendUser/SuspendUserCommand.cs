using Domain.Primitives;
using MediatR;

namespace Application.Features.Users.SuspendUser;

/// <summary>
/// Comando para suspender una cuenta de usuario
/// </summary>
public record SuspendUserCommand(
    Guid UserId,
    string Reason) : IRequest<Result>;
