using Microsoft.JSInterop;

namespace Presentation.BlazorserverApp.Components.Commons.Services;
/// <summary>
/// Implementación del constructor del servicio JS
/// </summary>
internal class JsServiceBuilder : IJsServiceBuilder
{
    private readonly IJSRuntime _jsRuntime;
    private readonly Dictionary<string, IJSObjectReference> _moduleCache;
    private IJSObjectReference? _currentModule;
    private string? _currentModulePath;

    public JsServiceBuilder(IJSRuntime jsRuntime, Dictionary<string, IJSObjectReference> moduleCache)
    {
        _jsRuntime = jsRuntime;
        _moduleCache = moduleCache;
    }

    /// <summary>
    /// Carga un módulo JavaScript o usa uno en caché si ya existe
    /// </summary>
    /// <param name="modulePath">Ruta al archivo JS</param>
    /// <returns>El constructor para encadenar métodos</returns>
    public IJsServiceBuilder WithModule(string modulePath)
    {
        if (string.IsNullOrEmpty(modulePath))
            throw new ArgumentException("La ruta del módulo no puede ser nula o vacía", nameof(modulePath));

        _currentModulePath = $"./js/{modulePath}.js";

        // Si el módulo ya está en caché, lo usamos directamente
        if (_moduleCache.TryGetValue(modulePath, out var cachedModule))
        {
            _currentModule = cachedModule;
            return this;
        }

        // El módulo se cargará de forma diferida en la primera invocación
        return this;
    }

    /// <summary>
    /// Asegura que el módulo esté cargado antes de usarlo
    /// </summary>
    private async Task EnsureModuleLoadedAsync()
    {
        if (_currentModule != null) return;

        if (string.IsNullOrEmpty(_currentModulePath))
            throw new InvalidOperationException("Debe llamar a WithModule antes de invocar funciones JS");

        try
        {
            _currentModule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", _currentModulePath);

            // Almacenamos el módulo en la caché
            if (_currentModule != null && !_moduleCache.ContainsKey(_currentModulePath))
            {
                _moduleCache[_currentModulePath] = _currentModule;
            }
        }
        catch (JSException ex)
        {
            throw new JSException($"Error al cargar el módulo JS '{_currentModulePath}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Ejecuta una función del módulo JS sin parámetros
    /// </summary>
    /// <param name="functionName">Nombre de la función JS a ejecutar</param>
    /// <returns>El constructor para encadenar métodos</returns>
    public async Task<IJsServiceBuilder> InvokeVoidAsync(string functionName)
    {
        await EnsureModuleLoadedAsync();

        try
        {
            await _currentModule!.InvokeVoidAsync(functionName);
        }
        catch (JSException ex)
        {
            throw new JSException($"Error al invocar la función JS '{functionName}': {ex.Message}", ex);
        }

        return this;
    }

    /// <summary>
    /// Ejecuta una función del módulo JS con parámetros
    /// </summary>
    /// <param name="functionName">Nombre de la función JS a ejecutar</param>
    /// <param name="args">Argumentos para la función</param>
    /// <returns>El constructor para encadenar métodos</returns>
    public async Task<IJsServiceBuilder> InvokeVoidAsync(string functionName, params object[] args)
    {
        await EnsureModuleLoadedAsync();

        try
        {
            await _currentModule!.InvokeVoidAsync(functionName, args);
        }
        catch (JSException ex)
        {
            throw new JSException($"Error al invocar la función JS '{functionName}': {ex.Message}", ex);
        }

        return this;
    }

    /// <summary>
    /// Ejecuta una función del módulo JS que devuelve un valor
    /// </summary>
    /// <typeparam name="TValue">Tipo del valor de retorno</typeparam>
    /// <param name="functionName">Nombre de la función JS a ejecutar</param>
    /// <returns>El valor devuelto por la función JS</returns>
    public async Task<TValue> InvokeAsync<TValue>(string functionName)
    {
        await EnsureModuleLoadedAsync();

        try
        {
            return await _currentModule!.InvokeAsync<TValue>(functionName);
        }
        catch (JSException ex)
        {
            throw new JSException($"Error al invocar la función JS '{functionName}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Ejecuta una función del módulo JS que devuelve un valor y recibe parámetros
    /// </summary>
    /// <typeparam name="TValue">Tipo del valor de retorno</typeparam>
    /// <param name="functionName">Nombre de la función JS a ejecutar</param>
    /// <param name="args">Argumentos para la función</param>
    /// <returns>El valor devuelto por la función JS</returns>
    public async Task<TValue> InvokeAsync<TValue>(string functionName, params object[] args)
    {
        await EnsureModuleLoadedAsync();

        try
        {
            return await _currentModule!.InvokeAsync<TValue>(functionName, args);
        }
        catch (JSException ex)
        {
            throw new JSException($"Error al invocar la función JS '{functionName}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene el módulo JS cargado
    /// </summary>
    /// <returns>Referencia al módulo JS</returns>
    public IJSObjectReference GetModule()
    {
        if (_currentModule == null)
            throw new InvalidOperationException("El módulo JS no ha sido cargado. Llame a WithModule primero.");

        return _currentModule;
    }
}