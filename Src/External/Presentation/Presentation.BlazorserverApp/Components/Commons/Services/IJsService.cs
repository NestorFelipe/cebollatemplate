namespace Presentation.BlazorserverApp.Components.Commons.Services;

/// <summary>
/// Interfaz para el servicio de JavaScript con patrón Builder
/// </summary>
public interface IJsService : IAsyncDisposable
{
    /// <summary>
    /// Inicia la construcción del servicio JS
    /// </summary>
    /// <returns>Constructor del servicio JS</returns>
    IJsServiceBuilder Create();
}