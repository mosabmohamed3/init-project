using REM.Domain.Common;

namespace REM.WebApi.Extensions;

public static class IAuthorizationPolicyConfig
{
    public static void AddPolicy(this IServiceCollection services)
    {
        services.AddAuthorization(opt =>
        {
            opt.AddPolicy(nameof(PolicyName.Admin), policy => policy.RequireRole(nameof(UserRole.SuperAdmin) , nameof(UserRole.Admin)));
            opt.AddPolicy(nameof(PolicyName.User), policy => policy.RequireRole(nameof(UserRole.Marketer), nameof(UserRole.Non_Marketer)));
        });
    }
}

public enum PolicyName
{
    Admin,
    User
}
