using FluentValidation;
using Infraestructure.Mappers.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Infraestructure.Mappers.Implements;

public class ValidFactory : IValidFactory
{

    private readonly IServiceProvider _serviceProvider;

    public ValidFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IValidator<T> GetValidator<T>() where T : class
    {
        return _serviceProvider.GetRequiredService<IValidator<T>>();
    }
}