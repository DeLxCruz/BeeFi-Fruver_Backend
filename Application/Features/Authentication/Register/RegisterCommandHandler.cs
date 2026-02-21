using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Authentication.Register
{
    public class RegisterCommandHandler
        : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEmailService _emailService;

        public RegisterCommandHandler(
            IApplicationDbContext context,
            IPasswordHasher passwordHasher,
            IEmailService emailService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
        }

        public async Task<Result<RegisterResponse>> Handle(
            RegisterCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Verificar si el email ya existe
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(
                    u => u.Email == request.Email.ToLowerInvariant(),
                    cancellationToken);

            if (existingUser != null)
            {
                return Result.Failure<RegisterResponse>(
                    new Error("User.EmailExists", "Ya existe un usuario con este email"));
            }

            // 2. Verificar si el teléfono ya existe
            var existingPhone = await _context.Users
                .FirstOrDefaultAsync(
                    u => u.PhoneNumber == request.PhoneNumber,
                    cancellationToken);

            if (existingPhone != null)
            {
                return Result.Failure<RegisterResponse>(
                    new Error("User.PhoneExists", "Ya existe un usuario con este teléfono"));
            }

            // 3. Crear usuario
            // FruverAliado requiere aprobación del administrador
            var requiresApproval = request.Type == UserType.FruverAliado;
            
            var user = User.Create(
                request.Email,
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                requiresApproval);

            // 4. Hash de contraseña
            var passwordHash = _passwordHasher.HashPassword(request.Password);
            user.SetPasswordHash(passwordHash);

            // 5. Asignar rol según tipo
            var roleName = request.Type switch
            {
                UserType.Cliente => "Cliente",
                UserType.FruverAliado => "FruverAliado",
                UserType.Empleado => "Empleado",
                _ => "Cliente"
            };

            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken);

            if (role == null)
            {
                return Result.Failure<RegisterResponse>(
                    new Error("Role.NotFound", $"El rol {roleName} no existe en el sistema"));
            }

            user.UserRoles.Add(UserRole.Create(user.Id, role.Id));

            // 6. Crear LoyaltyPoints para clientes
            if (request.Type == UserType.Cliente)
            {
                var loyaltyPoints = LoyaltyPoints.Create(user.Id);
                _context.LoyaltyPoints.Add(loyaltyPoints);
            }

            // 7. Guardar en base de datos
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            // 8. Enviar email según tipo de usuario
            if (request.Type == UserType.FruverAliado)
            {
                // Email de notificación de aprobación pendiente
                _ = _emailService.SendEmailAsync(
                    user.Email,
                    "Cuenta pendiente de aprobación - BeeFi",
                    $"Hola {user.FirstName},<br/><br/>Tu cuenta como Fruver Aliado está pendiente de aprobación por nuestro equipo. Te notificaremos cuando sea aprobada.<br/><br/>Saludos,<br/>Equipo BeeFi");
            }
            else
            {
                // Email de confirmación estándar
                _ = _emailService.SendConfirmationEmailAsync(
                    user.Email,
                    user.FirstName,
                    $"https://beefi.com/confirm-email?token={Guid.NewGuid()}");
            }

            // 9. Retornar response con mensaje apropiado
            var message = request.Type == UserType.FruverAliado
                ? "Tu cuenta está pendiente de aprobación por el administrador. Te notificaremos cuando sea aprobada."
                : "Registro exitoso. Por favor confirma tu email";

            return Result.Success(new RegisterResponse(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                roleName,
                message));
        }
    }
}