using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var authUrl = builder.Configuration["Keycloak:AuthorizationUrl"] 
              ?? "https://gsefact.girosystem.com:8433/auth/realms/gsEFact/protocol/openid-connect/auth";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Keycloak.Auth.Api", Version = "v1" });
    
    c.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));
    
    c.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://gsefact.girosystem.com:8443/realms/gsEFact/protocol/openid-connect/auth"),
                TokenUrl = new Uri("https://gsefact.girosystem.com:8443/realms/gsEFact/protocol/openid-connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "Access your information" },
                    { "profile", "Access your profile" }
                }
            }
        }
    });

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Keycloak"
                },
                Scheme = "Bearer",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            []
        }
    };

    c.AddSecurityRequirement(securityRequirement);
});

builder.Services.AddAuthorization();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Posar-ho a true en entorns de producció
        options.Authority = builder.Configuration["Authentication:Authority"];
        options.Audience = builder.Configuration["Authentication:Audience"];
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Authentication:ValidIssuer"] ?? $"{builder.Configuration["Authentication:Authority"]}/realms/gsEFact",
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true
        };
    });

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();

//    app.UseSwaggerUI();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.OAuthClientId("frontend-spa");
        c.OAuthAppName("My API - Swagger");
        c.OAuthUsePkce(); 
    });
//}

app.UseHttpsRedirection();

app.MapGet("users/me", (ClaimsPrincipal claimsPrincipal) =>
{
    return claimsPrincipal.Claims.ToDictionary(c => c.Type, c => c.Value);
}).RequireAuthorization();

app.UseAuthentication();

app.UseAuthorization();

app.Run();

