using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace REM.WebApi.MiddleWare;

public static class AuthenticationMiddleWare
{
    public static IServiceCollection RegisterAuthentication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        string secretKey = configuration.GetValue<string>("AuthSettings:TokenSecretKey")!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        services
            .AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(
                JwtBearerDefaults.AuthenticationScheme,
                x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                    };
                    x.Events = new JwtBearerEvents
                    {
                        // Custom logic to extract the JWT token from the query string for requests to SignalR hubs.
                        // This is necessary because WebSockets or server-sent events may not allow sending tokens in the Authorization header.
                        // Specifically checks for the 'access_token' query parameter in requests to the "/hubs/Notification" path.
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.Request.HttpContext.Request.Path;
                            if (
                                !string.IsNullOrEmpty(accessToken)
                                && (path.StartsWithSegments("/hubs/Notification"))
                            )
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        },
                    };
                }
            );

        return services;
    }
}
