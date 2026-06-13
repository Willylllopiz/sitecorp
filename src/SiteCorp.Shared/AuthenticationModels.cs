namespace SiteCorp.Shared;

public sealed record LoginRequest(
    string UserName,
    string Password);

public sealed record RefreshTokenRequest(
    string RefreshToken);

public sealed record LogoutRequest(
    string RefreshToken);

public sealed record AuthenticatedUser(
    int UserId,
    int CompanyId,
    string UserName,
    string FullName,
    string Email,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions);

public sealed record AuthTokenResponse(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt,
    AuthenticatedUser User);

