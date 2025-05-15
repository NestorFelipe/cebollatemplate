using Application.Contracts.Commons;
using FluentValidation;
using Infraestructure.Mappers.Implements;
using Infraestructure.Mappers.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Infraestructure.Mappers;

public static class InfraestructureMappersExtension
{
    public static IServiceCollection AddInfraestructureMappersExtension(this IServiceCollection services)
    {
        
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped<IGMapper, GMapper>();

        
        services.AddTransient<IValidFactory, ValidFactory>();
        services.AddScoped(typeof(IGValidators), typeof(GValidators));

        
        services.AddValidatorsFromAssembly(Assembly.Load("Infraestructure.Mappers"), ServiceLifetime.Scoped, null, true);

        return services;
    }
}