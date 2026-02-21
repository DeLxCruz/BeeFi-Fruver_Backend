using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Authentication.Logout
{
    public class LogoutCommandHandler
        : IRequestHandler<LogoutCommand, Result<LogoutResponse>>
    {
        private readonly IApplicationDbContext _context;

        public LogoutCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<LogoutResponse>> Handle(
            LogoutCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Verificar que el usuario existe
            var userExists = await _context.Users
                .AnyAsync(u => u.Id == request.UserId, cancellationToken);

            if (!userExists)
            {
                return Result.Failure<LogoutResponse>(
                    new Error("User.NotFound", "Usuario no encontrado"));
            }

            int tokensRevoked = 0;

            if (request.RevokeAllTokens)
            {
                // 2a. Revocar TODOS los refresh tokens del usuario (logout de todos los dispositivos)
                var allTokens = await _context.RefreshTokens
                    .Where(rt => rt.UserId == request.UserId && !rt.IsRevoked)
                    .ToListAsync(cancellationToken);

                foreach (var token in allTokens)
                {
                    token.Revoke();
                    tokensRevoked++;
                }
            }
            else if (!string.IsNullOrEmpty(request.RefreshToken))
            {
                // 2b. Revocar SOLO el refresh token específico (logout de un dispositivo)
                var specificToken = await _context.RefreshTokens
                    .FirstOrDefaultAsync(
                        rt => rt.Token == request.RefreshToken &&
                              rt.UserId == request.UserId &&
                              !rt.IsRevoked,
                        cancellationToken);

                if (specificToken == null)
                {
                    return Result.Failure<LogoutResponse>(
                        new Error("RefreshToken.NotFound", "Refresh token no encontrado o ya revocado"));
                }

                specificToken.Revoke();
                tokensRevoked = 1;
            }
            else
            {
                return Result.Failure<LogoutResponse>(
                    new Error("Logout.InvalidRequest", "Debe proporcionar un refresh token o indicar revocar todos"));
            }

            // 3. Guardar cambios
            await _context.SaveChangesAsync(cancellationToken);

            // 4. Retornar response
            return Result.Success(new LogoutResponse(
                request.UserId,
                tokensRevoked,
                request.RevokeAllTokens ?
                    "Se cerró sesión en todos los dispositivos" :
                    "Se cerró sesión en este dispositivo"
            ));
        }
    }
}