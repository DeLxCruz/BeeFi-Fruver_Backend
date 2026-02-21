using Domain.Primitives;
using MediatR;

namespace Application.Features.Users.RejectUser;

/// <summary>
/// Comando para rechazar una cuenta de usuario
/// </summary>
public record RejectUserCommand(
    Guid UserId,
    string Reason) : IRequest<Result>;
