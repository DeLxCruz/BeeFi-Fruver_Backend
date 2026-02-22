using FluentValidation;

namespace Application.Features.ReturnRequests.CreateReturnRequest;

public class CreateReturnRequestCommandValidator : AbstractValidator<CreateReturnRequestCommand>
{
    public CreateReturnRequestCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("El ID del pedido es requerido.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("El motivo de devolución es requerido.")
            .MaximumLength(1000).WithMessage("El motivo no puede superar 1000 caracteres.");
    }
}
