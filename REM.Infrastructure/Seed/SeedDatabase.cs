using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using REM.Application.Common.Interfaces;
using REM.Domain.Entities.Identity;
using REM.Infrastructure.Context;
using REM.Domain.Common;

namespace REM.Infrastructure.Seed;

public class SeedDatabase
{
    public static async Task Seed(IUnitOfWork<AppDbContext> unitOfWork, AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        await SeedRoleData(roleManager);
        await SeedUserData(context, userManager);
        await unitOfWork.SaveChangesAsync();
    }

    private static async Task SeedUserData(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _ = Guid.TryParse(ApplicationConstants.SuperAdminId, out var superAdmin);

        var user = new ApplicationUser
        {
            Id = superAdmin,
            Full_Name = "Super Admin",
            Email = "Programmer@programmer.com",
            NormalizedEmail = "PROGRAMMER@PROGRAMMER.COM",
            UserName = "Programmer",
            NormalizedUserName = "PROGRAMMER",
            Role = UserRole.SuperAdmin,
            Is_Active = true,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString("D"),
        };

        var set = context.Set<ApplicationUser>();
        if (!await set.AnyAsync(a => a.Id == superAdmin))
        {
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = passwordHasher.HashPassword(user, "P00000");
            await set.AddAsync(user);

            await context.SaveChangesAsync();
            await userManager.AddToRoleAsync(user, nameof(UserRole.SuperAdmin));
        }
    }

    private static async Task SeedRoleData(RoleManager<ApplicationRole> roleManager)
    {
        var roles = new List<ApplicationRole>();

        foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
            roles.Add(new ApplicationRole { Name = role.ToString() });

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
                await roleManager.CreateAsync(role);
        }
    }
}
