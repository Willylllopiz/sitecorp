using SiteCorp.Domain.Authentication;
using SiteCorp.Domain.Common;
using Xunit;

namespace SiteCorp.Domain.Tests.Authentication;

public sealed class RefreshTokenTests
{
    [Fact]
    public void Create_WhenExpirationIsNotInFuture_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new RefreshToken(
                userId: 1,
                tokenHash: "token-hash",
                expiresAt: DateTimeOffset.UtcNow.AddMinutes(-1),
                createdByIp: "127.0.0.1",
                userAgent: "tests"));
    }

    [Fact]
    public void Revoke_WhenTokenIsActive_MarksItAsInactive()
    {
        var token = new RefreshToken(
            userId: 1,
            tokenHash: "token-hash",
            expiresAt: DateTimeOffset.UtcNow.AddDays(7),
            createdByIp: "127.0.0.1",
            userAgent: "tests");

        token.Revoke("127.0.0.1");

        Assert.False(token.IsActive);
    }
}
