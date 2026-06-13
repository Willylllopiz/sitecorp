namespace SiteCorp.Application.Authentication;

public sealed class AuthenticationOptions
{
    public int RefreshTokenDays { get; set; } = 7;
}

