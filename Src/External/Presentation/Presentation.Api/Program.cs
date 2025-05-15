using Application.Dto.Commons;
using Infraestructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var Cors = builder.Configuration["Cors"]!.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

builder.Services.AddOptions()
        .Configure<SettingsApp>(builder.Configuration.GetSection(nameof(SettingsApp)));

builder.Services.AddHttpContextAccessor();
builder.Services.AddInfraestructureServicesExtension(builder.Configuration);

builder.Services.AddControllers()
            .AddJsonOptions(op =>
            {
                op.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                op.JsonSerializerOptions.WriteIndented = true;
            });

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FONTPLATA", Version = "v1" });

    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "JWT Authorization header using the Bearer scheme.",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                        {
                            {
                              new OpenApiSecurityScheme
                              {
                                  Reference = new OpenApiReference
                                  {
                                      Type = ReferenceType.SecurityScheme,
                                      Id = JwtBearerDefaults.AuthenticationScheme
                                  },
                                  Scheme = "oauth2",
                                  Name = JwtBearerDefaults.AuthenticationScheme,
                                  In = ParameterLocation.Header
                              },
                             new List<string>()
                            }
                        });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("cors", policyBuilder =>
    {
        policyBuilder.WithOrigins(Cors)
                     .AllowAnyHeader()
                     .AllowAnyMethod()
                     .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseCors("cors");

app.UseAuthorization();

app.MapControllers();

app.Run();
