using System.Net.Http.Json;
using SiteCorp.Shared;

namespace SiteCorp.Web.Clients;

public sealed class AuthenticationApiClient(HttpClient httpClient)
{
    public async Task<AuthTokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/auth/login", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Credenciales invalidas.");
        }

        return await response.Content.ReadFromJsonAsync<AuthTokenResponse>(cancellationToken)
            ?? throw new InvalidOperationException("El API no devolvio la sesion.");
    }

    public async Task LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await httpClient.PostAsJsonAsync("api/auth/logout", request, cancellationToken);
        }
        catch
        {
            // Logout local must continue even if the API is not reachable.
        }
    }
}

