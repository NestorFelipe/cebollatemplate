using Microsoft.JSInterop;

namespace Presentation.BlazorserverApp.Components.Commons.Services;

/// <summary>
/// Interfaz para el constructor del servicio JS
/// </summary>
public interface IJsServiceBuilder
{
    /// <summary>
    /// Carga un módulo JavaScript
    /// </summary>
    /// <param name="modulePath">Ruta al archivo JS</param>
    /// <returns>El constructor para encadenar métodos</returns>
    IJsServiceBuilder WithModule(string modulePath);

    /// <summary>
    /// Ejecuta una función del módulo JS sin parámetros
    /// </summary>
    /// <param name="functionName">Nombre de la función JS a ejecutar</param>
    /// <returns>El constructor para encadenar métodos</returns>
    Task<IJsServiceBuilder> InvokeVoidAsync(string functionName);

    /// <summary>
    /// Ejecuta una función del módulo JS con parámetros
    /// </summary>
    /// <param name="functionName">Nombre de la función JS a ejecutar</param>
    /// <param name="args">Argumentos para la función</param>
    /// <returns>El constructor para encadenar métodos</returns>
    Task<IJsServiceBuilder> InvokeVoidAsync(string functionName, params object[] args);

    /// <summary>
    /// Ejecuta una función del módulo JS que devuelve un valor
    /// </summary>
    /// <typeparam name="TValue">Tipo del valor de retorno</typeparam>
    /// <param name="functionName">Nombre de la función JS a ejecutar</param>
    /// <returns>El valor devuelto por la función JS</returns>
    Task<TValue> InvokeAsync<TValue>(string functionName);

    /// <summary>
    /// Ejecuta una función del módulo JS que devuelve un valor y recibe parámetros
    /// </summary>
    /// <typeparam name="TValue">Tipo del valor de retorno</typeparam>
    /// <param name="functionName">Nombre de la función JS a ejecutar</param>
    /// <param name="args">Argumentos para la función</param>
    /// <returns>El valor devuelto por la función JS</returns>
    Task<TValue> InvokeAsync<TValue>(string functionName, params object[] args);

    /// <summary>
    /// Obtiene el módulo JS cargado
    /// </summary>
    /// <returns>Referencia al módulo JS</returns>
    IJSObjectReference GetModule();
}