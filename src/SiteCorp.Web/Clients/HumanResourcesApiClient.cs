using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using SiteCorp.Shared;

namespace SiteCorp.Web.Clients;

public sealed class HumanResourcesApiClient(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider)
{
    public async Task<HumanResourcesSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default)
    {
        var metrics = await GetFromJsonAsync<DashboardMetrics>("api/dashboard", cancellationToken)
            ?? throw new InvalidOperationException("El API no devolvio metricas.");

        var employees = await GetFromJsonAsync<IReadOnlyList<Employee>>("api/employees", cancellationToken)
            ?? [];

        var positions = await GetFromJsonAsync<IReadOnlyList<Position>>("api/positions", cancellationToken)
            ?? [];

        var leaveRequests = await GetFromJsonAsync<IReadOnlyList<LeaveRequest>>("api/leave-requests", cancellationToken)
            ?? [];

        return new HumanResourcesSnapshot(metrics, employees, positions, leaveRequests, IsLive: true);
    }

    private async Task<T?> GetFromJsonAsync<T>(string requestUri, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        var accessToken = await GetAccessTokenAsync();

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
    }

    private async Task<string?> GetAccessTokenAsync()
    {
        var state = await authenticationStateProvider.GetAuthenticationStateAsync();
        return state.User.FindFirst("access_token")?.Value;
    }
}
