using SiteCorp.Domain.Authentication;

namespace SiteCorp.Application.Authentication;

public interface IAccessTokenService
{
    AuthAccessToken CreateAccessToken(User user, IReadOnlyList<string> roles, IReadOnlyList<string> permissions);
}

