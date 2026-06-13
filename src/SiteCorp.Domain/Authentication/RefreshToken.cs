using SiteCorp.Domain.Common;

namespace SiteCorp.Domain.Authentication;

public sealed class RefreshToken
{
    private RefreshToken()
    {
    }

    public RefreshToken(
        int userId,
        string tokenHash,
        DateTimeOffset expiresAt,
        string? createdByIp,
        string? userAgent)
    {
        if (userId <= 0)
        {
            throw new DomainException("El refresh token debe pertenecer a un usuario valido.");
        }

        if (expiresAt <= DateTimeOffset.UtcNow)
        {
            throw new DomainException("La expiracion del refresh token debe estar en el futuro.");
        }

        UserId = userId;
        TokenHash = RequireText(tokenHash, "El hash del refresh token es obligatorio.");
        ExpiresAt = expiresAt;
        CreatedAt = DateTimeOffset.UtcNow;
        CreatedByIp = CleanOptional(createdByIp);
        UserAgent = CleanOptional(userAgent);
    }

    public int Id { get; private set; }

    public int UserId { get; private set; }

    public string TokenHash { get; private set; } = string.Empty;

    public DateTimeOffset ExpiresAt { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public string? CreatedByIp { get; private set; }

    public DateTimeOffset? RevokedAt { get; private set; }

    public string? RevokedByIp { get; private set; }

    public int? ReplacedByTokenId { get; private set; }

    public string? UserAgent { get; private set; }

    public bool IsActive => RevokedAt is null && ExpiresAt > DateTimeOffset.UtcNow;

    public void Revoke(string? revokedByIp, int? replacedByTokenId = null)
    {
        if (RevokedAt is not null)
        {
            throw new DomainException("El refresh token ya fue revocado.");
        }

        RevokedAt = DateTimeOffset.UtcNow;
        RevokedByIp = CleanOptional(revokedByIp);
        ReplacedByTokenId = replacedByTokenId;
    }

    private static string RequireText(string value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(message);
        }

        return value.Trim();
    }

    private static string? CleanOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
