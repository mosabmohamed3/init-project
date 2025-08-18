using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using REM.Application.Common.Interfaces;
using REM.Domain.Entities.Identity;
using REM.Infrastructure.Context;

namespace REM.Infrastructure.Seed;

public static class DbInitializer
{
    public static async Task InitializeDatabase(this IApplicationBuilder app)
    {
        try
        {
            using var scope = app
                .ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

            await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await SeedDatabase.Seed(unitOfWork, context, userManager, roleManager);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex.InnerException);
        }
    }
}
