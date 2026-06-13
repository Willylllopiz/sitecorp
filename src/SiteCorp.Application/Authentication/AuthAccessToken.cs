namespace SiteCorp.Application.Authentication;

public sealed record AuthAccessToken(
    string Value,
    DateTimeOffset ExpiresAt);

