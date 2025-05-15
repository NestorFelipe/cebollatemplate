using Application.Contracts.Commons;
using Application.Contracts.Contracts.RestServices;
using Infraestructure.RestServices.Implement;
using Infraestructure.RestServices.Implement.LocalServices;
using Microsoft.Extensions.DependencyInjection;

namespace Infraestructure.RestServices;

public static class ServicesExtensionRest
{
    public static void AddServiceExtensionRest(this IServiceCollection services)
    {
        services.AddScoped<IRestBuilder, RestBuilder>();
        services.AddScoped<HttpClient>();
        services.AddScoped<IApirestManager, ApirestManager>();
    }

}
