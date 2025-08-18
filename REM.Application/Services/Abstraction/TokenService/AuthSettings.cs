namespace REM.Application.Services.Abstraction.TokenService;

public class AuthSettings
{
    public string TokenSecretKey { get; set; } = null!;
    public int AccessTokenExpiryInMinutes { get; set; }
    public int RefreshTokenExpiryInMinutes { get; set; }
}
