using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Authentication.Login
{
    public class LoginCommandHandler
        : IRequestHandler<LoginCommand, Result<LoginResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginCommandHandler(
            IApplicationDbContext context,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<Result<LoginResponse>> Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Buscar usuario con roles y suscripción BeeFi
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Include(u => u.BeeFiSubscription)
                    .ThenInclude(s => s!.Plan)
                .FirstOrDefaultAsync(
                    u => u.Email == request.Email.ToLowerInvariant(),
                    cancellationToken);

            if (user == null)
            {
                return Result.Failure<LoginResponse>(
                    new Error("Authentication.InvalidCredentials", "Email o contraseña incorrectos"));
            }

            // 2. Verificar contraseña
            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Result.Failure<LoginResponse>(
                    new Error("Authentication.InvalidCredentials", "Email o contraseña incorrectos"));
            }

            // 3. Verificar que el usuario esté activo
            if (!user.IsActive)
            {
                return Result.Failure<LoginResponse>(
                    new Error("Authentication.UserInactive", "Tu cuenta está inactiva. Contacta al administrador"));
            }

            // 4. Verificar estado de aprobación de la cuenta
            if (user.RequiresApproval())
            {
                return Result.Failure<LoginResponse>(
                    new Error("Authentication.PendingApproval", "Tu cuenta está pendiente de aprobación por el administrador"));
            }

            if (user.AccountStatus == AccountStatus.Rejected)
            {
                var rejectionMessage = string.IsNullOrEmpty(user.RejectionReason)
                    ? "Tu cuenta fue rechazada. Contacta al administrador para más información"
                    : $"Tu cuenta fue rechazada. Razón: {user.RejectionReason}";
                
                return Result.Failure<LoginResponse>(
                    new Error("Authentication.AccountRejected", rejectionMessage));
            }

            if (user.AccountStatus == AccountStatus.Suspended)
            {
                var suspensionMessage = string.IsNullOrEmpty(user.RejectionReason)
                    ? "Tu cuenta está suspendida. Contacta al administrador"
                    : $"Tu cuenta está suspendida. Razón: {user.RejectionReason}";
                
                return Result.Failure<LoginResponse>(
                    new Error("Authentication.AccountSuspended", suspensionMessage));
            }

            // 5. Generar JWT Token
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            var jwtToken = _jwtTokenGenerator.GenerateToken(user.Id, user.Email, roles);

            // 6. Generar Refresh Token
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
            var refreshTokenEntity = Domain.Entities.RefreshToken.Create(
                user.Id,
                refreshToken,
                expiryDays: 30,
                request.DeviceInfo,
                request.IpAddress);

            _context.RefreshTokens.Add(refreshTokenEntity);

            // 7. Actualizar último login
            user.UpdateLastLogin();
            await _context.SaveChangesAsync(cancellationToken);

            // 8. Construir response
            var hasBeeFi = user.BeeFiSubscription?.Status == SubscriptionStatus.Active;

            return Result.Success(new LoginResponse(
                jwtToken,
                refreshToken,
                DateTime.UtcNow.AddMinutes(30), // JWT expira en 30 min
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                roles,
                hasBeeFi,
                user.BeeFiSubscription?.Plan?.Name,
                user.BeeFiSubscription?.Plan?.DiscountPercentage ?? 0,
                user.BeeFiSubscription?.Plan?.BonusPointsMultiplier ?? 1
            ));
        }
    }
}