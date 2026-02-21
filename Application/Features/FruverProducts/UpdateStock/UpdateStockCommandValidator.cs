using FluentValidation;

namespace Application.Features.FruverProducts.UpdateStock;

public class UpdateStockCommandValidator : AbstractValidator<UpdateStockCommand>
{
    public UpdateStockCommandValidator()
    {
        RuleFor(x => x.FruverProductId)
            .NotEmpty().WithMessage("El Id del producto del fruver es requerido");

        RuleFor(x => x.NewStock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo");
    }
}
