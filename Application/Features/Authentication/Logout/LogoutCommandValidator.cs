using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.Features.Authentication.Logout
{
    public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
    {
        public LogoutCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId es requerido");

            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .When(x => !x.RevokeAllTokens)
                .WithMessage("Refresh token es requerido cuando no se revocan todos los tokens");
        }
    }
}