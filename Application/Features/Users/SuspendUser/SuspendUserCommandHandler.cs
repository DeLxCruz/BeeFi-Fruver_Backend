using Application.Common.Interfaces;
using Domain.Enums;
using Domain.Primitives;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.SuspendUser;

public class SuspendUserCommandValidator : AbstractValidator<SuspendUserCommand>
{
    public SuspendUserCommandValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Debe proporcionar una razón para la suspensión")
            .MaximumLength(500).WithMessage("La razón no puede exceder 500 caracteres");
    }
}

public class SuspendUserCommandHandler : IRequestHandler<SuspendUserCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public SuspendUserCommandHandler(
        IApplicationDbContext context,
        IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<Result> Handle(SuspendUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Buscar el usuario
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure(new Error("User.NotFound", "Usuario no encontrado"));
        }

        // 2. Verificar que no esté ya suspendido
        if (user.AccountStatus == AccountStatus.Suspended)
        {
            return Result.Failure(new Error("User.AlreadySuspended", "La cuenta ya está suspendida"));
        }

        // 3. Suspender la cuenta
        user.Suspend(request.Reason);

        await _context.SaveChangesAsync(cancellationToken);

        // 4. Enviar notificación por email
        _ = _emailService.SendEmailAsync(
            user.Email,
            "Tu cuenta ha sido suspendida - BeeFi",
            $"Hola {user.FirstName},<br/><br/>" +
            $"Tu cuenta ha sido suspendida temporalmente.<br/><br/>" +
            $"<strong>Razón:</strong> {request.Reason}<br/><br/>" +
            $"Para más información o apelar esta decisión, por favor contacta a nuestro equipo de soporte.<br/><br/>" +
            $"Saludos,<br/>Equipo BeeFi");

        return Result.Success();
    }
}
