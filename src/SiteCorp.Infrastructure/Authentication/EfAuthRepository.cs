using Microsoft.EntityFrameworkCore;
using SiteCorp.Application.Authentication;
using SiteCorp.Domain.Authentication;
using SiteCorp.Infrastructure.Data;

namespace SiteCorp.Infrastructure.Authentication;

public sealed class EfAuthRepository(SiteCorpDbContext dbContext) : IAuthRepository
{
    public Task<User?> GetUserByUserNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
    {
        return dbContext.Users.FirstOrDefaultAsync(
            user => user.NormalizedUserName == normalizedUserName,
            cancellationToken);
    }

    public Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return dbContext.Users.FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetRoleNamesAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await (
            from userRole in dbContext.UserRoles.AsNoTracking()
            join role in dbContext.Roles.AsNoTracking() on userRole.RoleId equals role.Id
            where userRole.UserId == userId && role.IsActive
            orderby role.Name
            select role.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetPermissionCodesAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await (
            from userRole in dbContext.UserRoles.AsNoTracking()
            join role in dbContext.Roles.AsNoTracking() on userRole.RoleId equals role.Id
            join rolePermission in dbContext.RolePermissions.AsNoTracking() on role.Id equals rolePermission.RoleId
            join permission in dbContext.Permissions.AsNoTracking() on rolePermission.PermissionId equals permission.Id
            where userRole.UserId == userId && role.IsActive
            select permission.Code)
            .Distinct()
            .OrderBy(permissionCode => permissionCode)
            .ToListAsync(cancellationToken);
    }

    public Task<RefreshToken?> GetRefreshTokenByHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return dbContext.RefreshTokens.FirstOrDefaultAsync(
            refreshToken => refreshToken.TokenHash == tokenHash,
            cancellationToken);
    }

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        await dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
