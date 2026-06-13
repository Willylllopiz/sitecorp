using SiteCorp.Domain.Authentication;
using SiteCorp.Domain.Common;
using Xunit;

namespace SiteCorp.Domain.Tests.Authentication;

public sealed class UserTests
{
    [Fact]
    public void Create_WhenEmailIsInvalid_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new User(
                companyId: 1,
                userName: "admin",
                firstName: "Laura",
                lastName: "Benitez",
                email: "laura",
                passwordHash: "hash"));
    }

    [Fact]
    public void RecordFailedLogin_WhenMaxAttemptsIsReached_LocksUser()
    {
        var user = CreateUser();

        user.RecordFailedLogin(maxAttempts: 2, lockoutDuration: TimeSpan.FromMinutes(15));
        user.RecordFailedLogin(maxAttempts: 2, lockoutDuration: TimeSpan.FromMinutes(15));

        Assert.NotNull(user.LockedUntil);
    }

    private static User CreateUser()
    {
        return new User(
            companyId: 1,
            userName: "admin",
            firstName: "Laura",
            lastName: "Benitez",
            email: "laura.benitez@sitecorp.local",
            passwordHash: "hash");
    }
}
