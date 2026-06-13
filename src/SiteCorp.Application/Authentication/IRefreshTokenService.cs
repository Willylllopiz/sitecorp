namespace SiteCorp.Application.Authentication;

public interface IRefreshTokenService
{
    string GenerateToken();

    string HashToken(string token);
}

