using System.Security.Cryptography;
using SiteCorp.Application.Authentication;

namespace SiteCorp.Infrastructure.Authentication;

public sealed class RefreshTokenService : IRefreshTokenService
{
    public string GenerateToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public string HashToken(string token)
    {
        var bytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}

