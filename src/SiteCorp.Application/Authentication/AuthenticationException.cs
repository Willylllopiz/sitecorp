namespace SiteCorp.Application.Authentication;

public sealed class AuthenticationException : Exception
{
    public AuthenticationException(string message)
        : base(message)
    {
    }
}

