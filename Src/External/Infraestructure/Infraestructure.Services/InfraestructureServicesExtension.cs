using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infraestructure.Mappers;
using Infraestructure.Persistence;
using Infraestructure.Persistence.Helpers;

namespace Infraestructure.Services;

public static class InfraestructureServicesExtension
{

    public static IServiceCollection AddInfraestructureServicesExtension(this IServiceCollection services, IConfiguration configuration)
    {
        services.AutoRegisterServices(configuration);
        services.AddInfraestructurePersistenceExtension(configuration);
        services.AddInfraestructureMappersExtension();
        
        
        InfraestructurePersistenceExtension._provider = services.BuildServiceProvider();

        return services;
    }

    public static IServiceCollection AutoRegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        return HelpersBuilders.AutoRegisterServices(services, configuration);
    }

}
