using Infraestructure.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Presentation.BlazorserverApp.Components;
using Presentation.BlazorserverApp.Components.Commons.Services;

var builder = WebApplication.CreateBuilder(args);

string[] initialScopes = builder.Configuration.GetValue<string>("MicrosoftGraph:Scopes")?.Split(' ')!;

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Configuración para Azure AD
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes) // Habilita adquisición de token
    .AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph")) // Configura el SDK de Graph
    .AddInMemoryTokenCaches(); // O usa un caché distribuido (Redis, SQL Server) en producción


builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser().Build();
});

// Necesario para Blazor y para que AuthorizeRouteView funcione correctamente
builder.Services.AddRazorPages(); // Si usas páginas Razor además de componentes
// Esto es CRUCIAL para que AuthorizeView y AuthorizeRouteView funcionen correctamente
// en los componentes Blazor.
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddServerSideBlazor(options =>
{
    options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(1); // El valor por defecto es 1 minuto.
    // Para depuración, podrías querer aumentarlo si estás depurando código JS lentamente.
    options.DetailedErrors = builder.Environment.IsDevelopment();
}).AddMicrosoftIdentityConsentHandler();


builder.Services.AddInfraestructureServicesExtension(builder.Configuration);

builder.Services.AddScoped<IJsService, JsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseRouting(); // Va antes de Authentication y Authorization
app.UseAuthentication(); // Habilita la autenticación
app.UseAuthorization();  // Habilita la autorización (importante para FallbackPolicy)
app.UseAntiforgery();
app.MapControllers(); // Para las UI de Microsoft Identity
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();