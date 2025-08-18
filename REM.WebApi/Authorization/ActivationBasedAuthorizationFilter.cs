using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using REM.Domain.Entities.Identity;

namespace REM.WebApi.Authorization;

public class ActivationBasedAuthorizationFilter(UserManager<ApplicationUser> userManager)
    : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var hasAllowAnonymous = context
            .ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>()
            .Any();

        if (hasAllowAnonymous)
            return;

        var claimIdentity = context.HttpContext.User.Identity as ClaimsIdentity;
        if (claimIdentity == null || !claimIdentity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var userId = Guid.Parse(claimIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await userManager
            .Users.Where(u => u.Id == userId)
            .Select(u => new { u.Is_Active, u.PhoneNumberConfirmed })
            .FirstOrDefaultAsync();

        if (user == null || !user.Is_Active || !user.PhoneNumberConfirmed)
            context.Result = new UnauthorizedResult();
    }
}
