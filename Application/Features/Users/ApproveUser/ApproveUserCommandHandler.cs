using Application.Common.Interfaces;
using Domain.Enums;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.ApproveUser;

public class ApproveUserCommandHandler : IRequestHandler<ApproveUserCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmailService _emailService;

    public ApproveUserCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IEmailService emailService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _emailService = emailService;
    }

    public async Task<Result> Handle(ApproveUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Buscar el usuario
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure(new Error("User.NotFound", "Usuario no encontrado"));
        }

        // 2. Verificar que esté pendiente
        if (user.AccountStatus != AccountStatus.Pending)
        {
            return Result.Failure(new Error(
                "User.NotPending",
                $"La cuenta no está pendiente de aprobación. Estado actual: {user.AccountStatus}"));
        }

        // 3. Aprobar la cuenta
        var currentAdminId = _currentUserService.UserId ?? Guid.Empty;
        if (currentAdminId == Guid.Empty)
        {
            return Result.Failure(new Error("Admin.NotAuthenticated", "No se pudo identificar al administrador actual"));
        }
        
        user.Approve(currentAdminId);

        await _context.SaveChangesAsync(cancellationToken);

        // 4. Enviar notificación por email
        _ = _emailService.SendEmailAsync(
            user.Email,
            "¡Tu cuenta ha sido aprobada! - BeeFi",
            $"Hola {user.FirstName},<br/><br/>" +
            $"¡Buenas noticias! Tu cuenta como Fruver Aliado ha sido aprobada.<br/><br/>" +
            $"Ya puedes iniciar sesión y comenzar a publicar tus productos.<br/><br/>" +
            $"<a href='https://beefi.com/login'>Iniciar Sesión</a><br/><br/>" +
            $"Saludos,<br/>Equipo BeeFi");

        return Result.Success();
    }
}
