using FluentValidation;

namespace Application.Features.Notifications.SendNotification;

public class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
{
    public SendNotificationCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("El ID del usuario es requerido.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título es requerido.")
            .MaximumLength(200).WithMessage("El título no puede superar 200 caracteres.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("El mensaje es requerido.")
            .MaximumLength(1000).WithMessage("El mensaje no puede superar 1000 caracteres.");
    }
}
