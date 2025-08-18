using Microsoft.AspNetCore.Identity;

namespace REM.Domain.Entities.Identity;

public class ApplicationRole : IdentityRole<Guid>
{
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = [];
}
