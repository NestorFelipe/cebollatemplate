using Domain.Entities.Atributes;
using Domain.Entities.BaseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Infraestructure.Persistence.Helpers;

public static class HelpersBuilders
{

    /// <summary>
    /// Metodo que se encarga de registrar los modelos DbSet<T>
    /// </summary>
    /// <param name="modelBuilder">Model enviado desde EntityFrameWork</param>
    /// <param name="namecontext">Nombre del contenedor de entidades por convension debe concluir con ...Context</param>
    public static void ScanToRegisterDbSet(ModelBuilder modelBuilder, string namecontext)
    {

        var entityTypes = Assembly.Load("Domain.Entities").GetTypes()
                                    .Where(t => t.IsClass &&
                                                !t.IsAbstract &&
                                                !t.IsGenericType &&
                                                t.FullName!.Contains(namecontext.Replace("Context", "")) &&
                                                typeof(AuditableBaseEntity).IsAssignableFrom(t) ||
                                                t.FullName!.Contains(namecontext.Replace("Context", "")) &&
                                                typeof(FunctionBaseTable).IsAssignableFrom(t));

        foreach (var entityType in entityTypes)
        {
            var vFields = entityType.GetProperties().Where(x => x.IsDefined(typeof(JsonType)) && x.PropertyType.IsClass);

            if (vFields.Count() > 0)
            {
                vFields.ToList().ForEach( prop =>
                {
                    modelBuilder.Entity(entityType, d => { d.OwnsOne(prop.PropertyType.FullName!, prop.Name, d => { d.ToJson(); }); });
                });
            }
            else
            {
                modelBuilder.Entity(entityType);
            }
        }

    }


    /// <summary>
    /// Metodo que se encarga de registar los servicios al contenedor de inyeccion de dependencias
    /// </summary>
    /// <param name="services">IServicecollection</param>
    /// <param name="configuration">IConfiguration</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AutoRegisterServices(IServiceCollection services, IConfiguration configuration)
    {

        var interfaceAssembly = Assembly.Load(configuration.GetSection("AssemblyConfig:InterfaceAssembly").Value!);
        var implementationAssembly = Assembly.Load(configuration.GetSection("AssemblyConfig:ImplementationAssembly").Value!);
        var interfaceNamespace = configuration.GetSection("AssemblyConfig:InterfaceNamespace").Value!;
        var implementationNamespace = configuration.GetSection("AssemblyConfig:ImplementationNamespace").Value!;

        var interfaceTypes = interfaceAssembly.GetExportedTypes();
        var implementationTypes = implementationAssembly.GetExportedTypes();

        // Filtra interfaces no genéricas que comienzan con "I"
        var matchingInterfaces = interfaceTypes
            .Where(t => t.Namespace != null && t.Namespace.StartsWith(interfaceNamespace) && t.IsInterface && !t.IsGenericType && t.Name.StartsWith("I"));

        foreach (var @interface in matchingInterfaces)
        {
            // Busca la implementación correspondiente
            var implementations = implementationTypes.Where(t => t.IsAssignableFrom(t)
                                                            && t.IsClass
                                                            && !t.IsGenericType
                                                            && t.Namespace!.StartsWith(implementationNamespace)
                                                            && t.GetInterfaces().Contains(@interface));

            foreach (var @implementation in implementations)
            {
                if (@implementation != null)
                    services.AddScoped(@interface, @implementation);
            }
        }

        return services;
    }

}
