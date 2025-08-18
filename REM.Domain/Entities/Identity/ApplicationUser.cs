using Microsoft.AspNetCore.Identity;
using REM.Domain.Common;
using REM.Domain.Entities.Auth;

namespace REM.Domain.Entities.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    // main properties
    public string Full_Name { get; set; } = null!;
    public UserRole Role { get; set; }
    public string? Fal_License { get; set; }
    public string? ID_Card { get; set; }
    public string? CommercialRegistration_Number { get; set; }

    // additional properties
    public DateTime Suspended_To { get; set; }
    public bool Is_Active { get; set; } = true;
    public bool Is_Deleted { get; set; } = false;
    public Guid Created_By { get; set; }
    public DateTime Created_At { get; set; }
    public Guid? Modified_By { get; set; }
    public DateTime? Modified_At { get; set; }

    // navigation properties
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<ApplicationUserRole> UserRoles { get; set; } = [];
}
