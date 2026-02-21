using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Enums;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Authentication.RefreshToken
{
    public class RefreshTokenCommandHandler
        : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public RefreshTokenCommandHandler(
            IApplicationDbContext context,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<Result<RefreshTokenResponse>> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Buscar el refresh token en la base de datos
            var storedToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                    .ThenInclude(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                .Include(rt => rt.User)
                    .ThenInclude(u => u.BeeFiSubscription)
                        .ThenInclude(s => s!.Plan)
                .FirstOrDefaultAsync(
                    rt => rt.Token == request.RefreshToken,
                    cancellationToken);

            if (storedToken == null)
            {
                return Result.Failure<RefreshTokenResponse>(
                    new Error("RefreshToken.Invalid", "Refresh token inválido"));
            }

            // 2. Verificar que el token no esté revocado
            if (storedToken.IsRevoked)
            {
                return Result.Failure<RefreshTokenResponse>(
                    new Error("RefreshToken.Revoked", "El refresh token ha sido revocado"));
            }

            // 3. Verificar que el token no esté expirado
            if (storedToken.IsExpired)
            {
                return Result.Failure<RefreshTokenResponse>(
                    new Error("RefreshToken.Expired", "El refresh token ha expirado"));
            }

            // 4. Verificar que el usuario esté activo
            var user = storedToken.User;
            if (!user.IsActive)
            {
                return Result.Failure<RefreshTokenResponse>(
                    new Error("User.Inactive", "Usuario inactivo"));
            }

            // 5. Generar nuevo JWT Token
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            var newJwtToken = _jwtTokenGenerator.GenerateToken(user.Id, user.Email, roles);

            // 6. Generar nuevo Refresh Token (rotación de tokens)
            var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();
            var newRefreshTokenEntity = Domain.Entities.RefreshToken.Create(
                user.Id,
                newRefreshToken,
                expiryDays: 30,
                request.DeviceInfo ?? storedToken.DeviceInfo,
                request.IpAddress ?? storedToken.IpAddress);

            _context.RefreshTokens.Add(newRefreshTokenEntity);

            // 7. Revocar el refresh token antiguo (rotación)
            storedToken.Revoke(newRefreshToken);

            // 8. Actualizar último login
            user.UpdateLastLogin();

            await _context.SaveChangesAsync(cancellationToken);

            // 9. Construir response
            var hasBeeFi = user.BeeFiSubscription?.Status == SubscriptionStatus.Active;

            return Result.Success(new RefreshTokenResponse(
                newJwtToken,
                newRefreshToken,
                DateTime.UtcNow.AddMinutes(30), // JWT expira en 30 min
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                roles,
                hasBeeFi,
                user.BeeFiSubscription?.Plan?.Name
            ));
        }
    }
}