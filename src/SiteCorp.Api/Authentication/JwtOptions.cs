namespace SiteCorp.Api.Authentication;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "SiteCorp";

    public string Audience { get; set; } = "SiteCorp.Client";

    public string SigningKey { get; set; } = "sitecorp-local-development-signing-key-change-before-production";

    public int AccessTokenMinutes { get; set; } = 60;
}

