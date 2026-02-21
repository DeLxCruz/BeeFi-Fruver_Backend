using FluentValidation;

namespace Application.Features.Addresses.CreateAddress;

public class CreateAddressCommandValidator : AbstractValidator<CreateAddressCommand>
{
    public CreateAddressCommandValidator()
    {
        RuleFor(x => x.ZoneId)
            .NotEmpty().WithMessage("La zona es requerida");

        RuleFor(x => x.AliasName)
            .NotEmpty().WithMessage("El alias de la dirección es requerido")
            .MaximumLength(50).WithMessage("El alias no puede superar 50 caracteres");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("La calle es requerida")
            .MaximumLength(200).WithMessage("La calle no puede superar 200 caracteres");

        RuleFor(x => x.HouseNumber)
            .NotEmpty().WithMessage("El número de casa/apto es requerido")
            .MaximumLength(20).WithMessage("El número no puede superar 20 caracteres");

        RuleFor(x => x.Neighborhood)
            .NotEmpty().WithMessage("El barrio/información adicional es requerida")
            .MaximumLength(200).WithMessage("El barrio no puede superar 200 caracteres");
    }
}
