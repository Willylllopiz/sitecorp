using SiteCorp.Domain.Authentication;

namespace SiteCorp.Application.Authentication;

public interface IAuthRepository
{
    Task<User?> GetUserByUserNameAsync(string normalizedUserName, CancellationToken cancellationToken = default);

    Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetRoleNamesAsync(int userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetPermissionCodesAsync(int userId, CancellationToken cancellationToken = default);

    Task<RefreshToken?> GetRefreshTokenByHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

