using Microsoft.AspNetCore.Identity;
using SiteCorp.Application.Authentication;
using SiteCorp.Domain.Authentication;

namespace SiteCorp.Infrastructure.Authentication;

public sealed class IdentityPasswordHashService : IPasswordHashService
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public bool Verify(User user, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}

