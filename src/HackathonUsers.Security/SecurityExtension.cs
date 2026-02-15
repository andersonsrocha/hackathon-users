using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using HackathonUsers.Data;
using HackathonUsers.Domain.Models;
using HackathonUsers.Security.Interfaces;
using HackathonUsers.Security.Models;
using HackathonUsers.Security.Services;

namespace HackathonUsers.Security;

public static class SecurityExtension
{
    public static void AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ServiceClientOptions>(options => configuration.GetSection("ServiceClients").Bind(options.Clients));
        // Adicionar o Identity
        services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 0;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<HackathonUsersContext>()
            .AddDefaultTokenProviders();
        // Configurar a autenticação JWT
        services.AddTransient<IJwtService, JwtService>();
        services.AddSingleton<IServiceClientValidation, ServiceClientValidation>();
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy("Admin", policy => policy.RequireAssertion(ctx =>
                ctx.User.IsInRole("Admin") ||
                ctx.User.HasClaim("token_type", "service")
            ))
            .AddPolicy("User", policy => policy.RequireAssertion(ctx =>
                ctx.User.IsInRole("Admin") ||
                ctx.User.IsInRole("User") ||
                ctx.User.HasClaim("token_type", "service")
            ));
    }
}