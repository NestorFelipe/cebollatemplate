using Application.Dto.Models.Appsgp;
using Domain.Entities.Appsgp;
using FluentValidation;

namespace Infraestructure.Mappers.Validators.Appsgp;

public class CarCarteraValidator : AbstractValidator<CarCarteraDto>
{
    public CarCarteraValidator()
    {

        RuleFor(x => x.CodigoCartera)
         .NotNull().WithMessage($"{nameof(CarCartera.CodigoCartera)} es obligatorio");
         
    }
}
