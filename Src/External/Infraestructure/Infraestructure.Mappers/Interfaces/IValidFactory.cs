using FluentValidation;

namespace Infraestructure.Mappers.Interfaces;

public interface IValidFactory
{
    IValidator<T> GetValidator<T>() where T : class;
}
