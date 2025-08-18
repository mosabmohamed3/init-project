using REM.Domain.Entities.Identity;

namespace REM.Application.Services.Abstraction.TokenService;

public interface ITokenService
{
    Task<TokenDto> GenerateToken(ApplicationUser user);
}
