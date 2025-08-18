using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using REM.Application.Common.Interfaces;
using REM.Domain.Entities.Identity;
using REM.Infrastructure.Context;
using REM.Infrastructure.Logging;
using REM.Infrastructure.RateLimiting;

namespace REM.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        return services
            .AddRateLimit(config)
            .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
            .AddHttpClient()
            .AddLocalization()
            .Configure<RequestLocalizationOptions>(_ => LocalizationConfig.GetLocalizationOptions())
            .RegisterUnitOfWork<AppDbContext>()
            .RegisterIdentity()
            .AddSerilogLogging()
            .ConfigureHangfire(config)
            .AddServices()
            .RegisterSignalR();
    }

    public static IServiceCollection RegisterUnitOfWork<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
        services.AddScoped<IUnitOfWork, UnitOfWork<TContext>>();
        services.AddScoped<IUnitOfWork<TContext>, UnitOfWork<TContext>>();
        return services;
    }

    public static IServiceCollection RegisterIdentity(this IServiceCollection services)
    {
        var builder = services.AddIdentityCore<ApplicationUser>(opt =>
        {
            opt.Password.RequireDigit = true;
            opt.Password.RequiredLength = 6;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequiredUniqueChars = 0;
            opt.Password.RequireLowercase = false;
            opt.User.RequireUniqueEmail = false;
            opt.SignIn.RequireConfirmedEmail =
                opt.SignIn.RequireConfirmedPhoneNumber =
                opt.SignIn.RequireConfirmedAccount =
                    false;
        });
        var identityBuilder = new IdentityBuilder(
            builder.UserType,
            typeof(ApplicationRole),
            builder.Services
        );
        identityBuilder.AddEntityFrameworkStores<AppDbContext>();
        identityBuilder.AddSignInManager<SignInManager<ApplicationUser>>();
        identityBuilder.AddRoleManager<RoleManager<ApplicationRole>>();
        services.TryAddSingleton(TimeProvider.System);

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services;
    }
}

public static class Startup
{
    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        return app.UseRateLimit()
            .UseRequestCulture()
            .UseStaticFiles()
            .UseCors("CorsPolicy")
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseHangfireDashboard()
            .UseSignalR();
    }

    private static IApplicationBuilder UseRequestCulture(this IApplicationBuilder app)
    {
        var localizationOptions = LocalizationConfig.GetLocalizationOptions();
        app.UseRequestLocalization(localizationOptions);
        return app;
    }
}

public static class LocalizationConfig
{
    public static RequestLocalizationOptions GetLocalizationOptions()
    {
        var supportedCultures = new[] { "en", "ar" };
        return new RequestLocalizationOptions()
            .SetDefaultCulture(supportedCultures[0])
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);
    }
}

public static class HangfireConfig
{
    public static IServiceCollection ConfigureHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        services = services.AddHangfire(config =>
        {
            config.UseSqlServerStorage(
                configuration.GetConnectionString("DefaultConnection"),
                new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                });

            config.UseSerializerSettings(new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

        });

        services.AddHangfireServer(
            options =>
            {
                options.ServerName = "REM Hangfire Server";
                options.Queues = ["notifications"];
            }
        );

        return services;
    }

    public static IApplicationBuilder UseHangfireDashboard(this IApplicationBuilder app)
    {
        return app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            DashboardTitle = "Real state Marketing Dashboard",
            Authorization = [ new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
            {
                RequireSsl = false,
                SslRedirect = false,
                LoginCaseSensitive = true,
                Users = [
                    new BasicAuthAuthorizationUser
                    {
                        Login = "admin",
                        PasswordClear = "admin"
                    }
                ],
            })
            ],
            StatsPollingInterval = 60_000
        });
    }
    public static IServiceCollection RegisterSignalR(this IServiceCollection services)
    {
        services.AddSignalR();
        return services;
    }

    public static IApplicationBuilder UseSignalR(this IApplicationBuilder app)
    {
        return app;
    }
}
