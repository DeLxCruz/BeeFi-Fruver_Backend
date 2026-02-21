using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.Features.Authentication.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es requerido")
                .EmailAddress().WithMessage("El email no es válido")
                .MaximumLength(100).WithMessage("El email no puede exceder 100 caracteres");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es requerida")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres")
                .Matches("[A-Z]").WithMessage("Debe contener al menos una letra mayúscula")
                .Matches("[a-z]").WithMessage("Debe contener al menos una letra minúscula")
                .Matches("[0-9]").WithMessage("Debe contener al menos un número")
                .Matches("[^a-zA-Z0-9]").WithMessage("Debe contener al menos un carácter especial");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("El nombre es requerido")
                .MaximumLength(50).WithMessage("El nombre no puede exceder 50 caracteres");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("El apellido es requerido")
                .MaximumLength(50).WithMessage("El apellido no puede exceder 50 caracteres");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("El teléfono es requerido")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Formato de teléfono inválido")
                .MaximumLength(20);

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Tipo de usuario inválido");
        }
    }
}