using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Authentication.ValidateToken
{
    public class ValidateTokenQueryHandler
        : IRequestHandler<ValidateTokenQuery, Result<ValidateTokenResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public ValidateTokenQueryHandler(
            IApplicationDbContext context,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<Result<ValidateTokenResponse>> Handle(
            ValidateTokenQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Validar el token JWT
            var principal = _jwtTokenGenerator.ValidateToken(request.Token);

            if (principal == null)
            {
                return Result.Failure<ValidateTokenResponse>(
                    new Error("Token.Invalid", "Token inválido o expirado"));
            }

            // 2. Extraer UserId del token
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Result.Failure<ValidateTokenResponse>(
                    new Error("Token.InvalidClaims", "Claims del token inválidos"));
            }

            // 3. Verificar que el usuario existe y está activo
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
            {
                return Result.Failure<ValidateTokenResponse>(
                    new Error("User.NotFound", "Usuario no encontrado"));
            }

            if (!user.IsActive)
            {
                return Result.Failure<ValidateTokenResponse>(
                    new Error("User.Inactive", "Usuario inactivo"));
            }

            // 4. Retornar información del usuario
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

            return Result.Success(new ValidateTokenResponse(
                userId,
                user.Email,
                user.FirstName,
                user.LastName,
                roles,
                true
            ));
        }
    }
}