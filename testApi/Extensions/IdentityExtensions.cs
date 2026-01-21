using Application.Abstractions.Utils;
using infrastructure.Utils.JwtService;
using infrastructure.Utils.PasswodHashService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using testApi.WebUtils.JwtClaimUtil;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdentityExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

        services.AddScoped<JwtClaimUtil>();
        services.AddSingleton<IJwtService, JwtService>();
        services.AddScoped<IPasswordHashService, PasswordHashService>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
            };
        });

        services.AddAuthorization();

        return services;
    }
}