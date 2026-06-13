using SiteCorp.Domain.Authentication;

namespace SiteCorp.Application.Authentication;

public interface IPasswordHashService
{
    bool Verify(User user, string password);
}

