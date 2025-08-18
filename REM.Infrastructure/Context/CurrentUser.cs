using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using REM.Application.Common.Interfaces;
using REM.Domain.Common;

namespace REM.Infrastructure.Context;

public class CurrentUser(IHttpContextAccessor httpContext) : ICurrentUser
{
    private readonly HttpContext _httpContext = httpContext.HttpContext!;

    public Guid GetUserId()
    {
        Guid.TryParse(ApplicationConstants.SuperAdminId, out var superAdmin);
        if (_httpContext?.User is null)
            return superAdmin;

        var idValue = _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(idValue))
            return superAdmin;
        return Guid.Parse(idValue);
    }

    public string? GetPhoneNumber()
    {
        return _httpContext.User.FindFirstValue(ClaimTypes.MobilePhone);
    }

    public UserRole GetUserRole()
    {
        string roleValue = _httpContext.User.FindFirstValue(ClaimTypes.Role)!;
        UserRole userRole = Enum.Parse<UserRole>(roleValue);
        return userRole;
    }
}
