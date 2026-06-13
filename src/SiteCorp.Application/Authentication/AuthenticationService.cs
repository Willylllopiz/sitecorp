using Microsoft.Extensions.Options;
using SiteCorp.Domain.Authentication;
using SiteCorp.Shared;

namespace SiteCorp.Application.Authentication;

public sealed class AuthenticationService(
    IAuthRepository repository,
    IPasswordHashService passwordHashService,
    IRefreshTokenService refreshTokenService,
    IAccessTokenService accessTokenService,
    IOptions<AuthenticationOptions> options)
{
    public async Task<AuthTokenResponse> LoginAsync(LoginRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new AuthenticationException("Usuario o password incorrectos.");
        }

        var normalizedUserName = request.UserName.Trim().ToUpperInvariant();
        var user = await repository.GetUserByUserNameAsync(normalizedUserName, cancellationToken);

        if (user is null || !user.IsActive || (user.LockedUntil is not null && user.LockedUntil > DateTimeOffset.UtcNow))
        {
            throw new AuthenticationException("Usuario o password incorrectos.");
        }

        if (!passwordHashService.Verify(user, request.Password))
        {
            user.RecordFailedLogin(maxAttempts: 5, lockoutDuration: TimeSpan.FromMinutes(15));
            await repository.SaveChangesAsync(cancellationToken);
            throw new AuthenticationException("Usuario o password incorrectos.");
        }

        user.RecordSuccessfulLogin();
        var response = await CreateTokenResponseAsync(user, ipAddress, userAgent, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return response;
    }

    public async Task<AuthTokenResponse> RefreshAsync(RefreshTokenRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            throw new AuthenticationException("Refresh token invalido.");
        }

        var tokenHash = refreshTokenService.HashToken(request.RefreshToken);
        var storedToken = await repository.GetRefreshTokenByHashAsync(tokenHash, cancellationToken);

        if (storedToken is null || !storedToken.IsActive)
        {
            throw new AuthenticationException("Refresh token invalido.");
        }

        var user = await repository.GetUserByIdAsync(storedToken.UserId, cancellationToken);

        if (user is null || !user.IsActive)
        {
            throw new AuthenticationException("Usuario invalido.");
        }

        storedToken.Revoke(ipAddress);
        var response = await CreateTokenResponseAsync(user, ipAddress, userAgent, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return response;
    }

    public async Task LogoutAsync(LogoutRequest request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return;
        }

        var tokenHash = refreshTokenService.HashToken(request.RefreshToken);
        var storedToken = await repository.GetRefreshTokenByHashAsync(tokenHash, cancellationToken);

        if (storedToken is null || !storedToken.IsActive)
        {
            return;
        }

        storedToken.Revoke(ipAddress);
        await repository.SaveChangesAsync(cancellationToken);
    }

    public async Task<AuthenticatedUser> GetCurrentUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetUserByIdAsync(userId, cancellationToken)
            ?? throw new AuthenticationException("Usuario invalido.");

        var roles = await repository.GetRoleNamesAsync(user.Id, cancellationToken);
        var permissions = await repository.GetPermissionCodesAsync(user.Id, cancellationToken);

        return MapUser(user, roles, permissions);
    }

    private async Task<AuthTokenResponse> CreateTokenResponseAsync(User user, string? ipAddress, string? userAgent, CancellationToken cancellationToken)
    {
        var roles = await repository.GetRoleNamesAsync(user.Id, cancellationToken);
        var permissions = await repository.GetPermissionCodesAsync(user.Id, cancellationToken);
        var accessToken = accessTokenService.CreateAccessToken(user, roles, permissions);
        var refreshTokenValue = refreshTokenService.GenerateToken();
        var refreshTokenExpiresAt = DateTimeOffset.UtcNow.AddDays(Math.Max(1, options.Value.RefreshTokenDays));
        var refreshToken = new RefreshToken(
            user.Id,
            refreshTokenService.HashToken(refreshTokenValue),
            refreshTokenExpiresAt,
            ipAddress,
            userAgent);

        await repository.AddRefreshTokenAsync(refreshToken, cancellationToken);

        return new AuthTokenResponse(
            accessToken.Value,
            accessToken.ExpiresAt,
            refreshTokenValue,
            refreshTokenExpiresAt,
            MapUser(user, roles, permissions));
    }

    private static AuthenticatedUser MapUser(User user, IReadOnlyList<string> roles, IReadOnlyList<string> permissions)
    {
        return new AuthenticatedUser(
            user.Id,
            user.CompanyId,
            user.UserName,
            user.FullName,
            user.Email,
            roles,
            permissions);
    }
}
