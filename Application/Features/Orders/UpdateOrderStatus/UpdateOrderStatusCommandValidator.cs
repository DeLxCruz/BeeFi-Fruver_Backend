using FluentValidation;

namespace Application.Features.Orders.UpdateOrderStatus;

public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("El ID del pedido es requerido");

        RuleFor(x => x.NewStatus)
            .IsInEnum().WithMessage("El estado proporcionado no es válido");
    }
}
