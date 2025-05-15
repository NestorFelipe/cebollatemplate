using Application.Contracts.Commons;
using Infraestructure.Persistence.Contexts;
using Infraestructure.Persistence.FactoryRepo;
using Infraestructure.Persistence.Logs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Web;

namespace Infraestructure.Persistence;

public static class InfraestructurePersistenceExtension
{
    public static ServiceProvider? _provider { get; set; }

    public static IServiceCollection AddInfraestructurePersistenceExtension(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddPooledDbContextFactory<AppsgpContext>(options =>
                                               options.UseNpgsql(configuration.GetConnectionString("Appsgp") ??
                                               throw new InvalidOperationException("No existe conexcion con el servidor de base de datos"))
                                               .UseSnakeCaseNamingConvention());

        services.AddPooledDbContextFactory<AppblazingurlsContext>(options =>
                                               options.UseSqlServer(configuration.GetConnectionString("Appblazingurls") ??
                                               throw new InvalidOperationException("No existe conexcion con el servidor de base de datos"))
                                               .UseSnakeCaseNamingConvention());

        services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));
        services.AddScoped(typeof(IRepository<>), typeof(BuildRepository<>));
        services.AddScoped<ISecurity, Security>();

        return services;
    }


    public static Logger LoggerInit()
    {
        return LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
    }

}
