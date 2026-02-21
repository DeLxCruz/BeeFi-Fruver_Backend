using FluentValidation;

namespace Application.Features.FruverProducts.UpdateFruverProduct;

public class UpdateFruverProductCommandValidator : AbstractValidator<UpdateFruverProductCommand>
{
    public UpdateFruverProductCommandValidator()
    {
        RuleFor(x => x.FruverProductId)
            .NotEmpty().WithMessage("El Id del producto del fruver es requerido");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("El precio debe ser mayor a cero");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo");

        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(0, 100).WithMessage("El descuento debe estar entre 0 y 100");

        RuleFor(x => x.BeeFiExclusiveDiscount)
            .InclusiveBetween(0, 100).WithMessage("El descuento BeeFi debe estar entre 0 y 100");
    }
}
