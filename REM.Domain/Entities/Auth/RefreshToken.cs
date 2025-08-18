using REM.Domain.Entities.Identity;

namespace REM.Domain.Entities.Auth;

public class RefreshToken : BaseEntity
{
    public required string Token { get; set; }
    public DateTime Expires_On { get; set; }
    public bool Is_Expired => DateTime.UtcNow >= Expires_On;
    public Guid User_Id { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;
}
