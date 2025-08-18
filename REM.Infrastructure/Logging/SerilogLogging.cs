using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace REM.Infrastructure.Logging;

public static class SerilogLogging
{
    public static IServiceCollection AddSerilogLogging(this IServiceCollection services)
    {
        var logger = new LoggerConfiguration()
        .MinimumLevel.Error()
        .WriteTo.File(
            "logs/REM-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 7)
        .CreateLogger();

        return services
            .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(logger))
            .AddSingleton(logger);
    }
}