using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace REM.Infrastructure.RateLimiting;

public static class RateLimitConfig
{
    public static IServiceCollection AddRateLimit(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services.AddMemoryCache();
        services.Configure<IpRateLimitOptions>(config.GetSection("IpRateLimiting"));
        services.AddInMemoryRateLimiting();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        return services;
    }

    public static IApplicationBuilder UseRateLimit(this IApplicationBuilder app) =>
        app.UseIpRateLimiting();
}
