using Microsoft.JSInterop;

namespace Presentation.BlazorserverApp.Components.Commons.Services;

/// <summary>
/// Implementación del servicio JS con patrón Builder
/// </summary>
public class JsService : IJsService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly Dictionary<string, IJSObjectReference> _moduleCache;

    public JsService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        _moduleCache = new Dictionary<string, IJSObjectReference>();
    }

    /// <summary>
    /// Crea una nueva instancia del constructor del servicio JS
    /// </summary>
    /// <returns>Constructor del servicio JS</returns>
    public IJsServiceBuilder Create()
    {
        return new JsServiceBuilder(_jsRuntime, _moduleCache);
    }

    /// <summary>
    /// Libera los recursos utilizados por el servicio
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        foreach (var module in _moduleCache.Values)
        {
            try
            {
                await module.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Ignora si el runtime de JS ya está desconectado
            }
            catch (ObjectDisposedException)
            {
                // Ignora si el módulo ya fue desechado
            }
        }

        _moduleCache.Clear();
        GC.SuppressFinalize(this);
    }
}