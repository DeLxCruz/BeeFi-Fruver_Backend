using FluentValidation;

namespace Application.Features.Notifications.RegisterDeviceToken;

public class RegisterDeviceTokenCommandValidator : AbstractValidator<RegisterDeviceTokenCommand>
{
    public RegisterDeviceTokenCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("El token del dispositivo es requerido.")
            .MaximumLength(500).WithMessage("El token no puede superar 500 caracteres.");
    }
}
