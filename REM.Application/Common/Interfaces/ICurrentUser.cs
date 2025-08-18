using REM.Domain.Common;

namespace REM.Application.Common.Interfaces;

public interface ICurrentUser
{
    Guid GetUserId();
    string? GetPhoneNumber();
    UserRole GetUserRole();
}
