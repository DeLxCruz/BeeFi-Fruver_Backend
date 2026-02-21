using Application.Common.Interfaces;
using Domain.Enums;
using Domain.Primitives;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.RejectUser;

public class RejectUserCommandValidator : AbstractValidator<RejectUserCommand>
{
    public RejectUserCommandValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Debe proporcionar una razón para el rechazo")
            .MaximumLength(500).WithMessage("La razón no puede exceder 500 caracteres");
    }
}

public class RejectUserCommandHandler : IRequestHandler<RejectUserCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmailService _emailService;

    public RejectUserCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IEmailService emailService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _emailService = emailService;
    }

    public async Task<Result> Handle(RejectUserCommand request, CancellationToken cancellationToken)
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

        // 3. Rechazar la cuenta
        var currentAdminId = _currentUserService.UserId ?? Guid.Empty;
        if (currentAdminId == Guid.Empty)
        {
            return Result.Failure(new Error("Admin.NotAuthenticated", "No se pudo identificar al administrador actual"));
        }
        
        user.Reject(request.Reason, currentAdminId);

        await _context.SaveChangesAsync(cancellationToken);

        // 4. Enviar notificación por email
        _ = _emailService.SendEmailAsync(
            user.Email,
            "Actualización de tu solicitud - BeeFi",
            $"Hola {user.FirstName},<br/><br/>" +
            $"Lamentamos informarte que tu solicitud para ser Fruver Aliado no ha sido aprobada.<br/><br/>" +
            $"<strong>Razón:</strong> {request.Reason}<br/><br/>" +
            $"Si tienes preguntas o deseas apelar esta decisión, por favor contacta a nuestro equipo de soporte.<br/><br/>" +
            $"Saludos,<br/>Equipo BeeFi");

        return Result.Success();
    }
}
