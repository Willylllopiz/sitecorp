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

    public Task<IReadOnlyList<OrganizationEntityResponse>> GetOrganizationEntitiesAsync(CancellationToken cancellationToken = default)
    {
        return GetRequiredFromJsonAsync<IReadOnlyList<OrganizationEntityResponse>>("api/organization/entities", cancellationToken);
    }

    public Task<OrganizationEntityResponse> CreateBusinessGroupAsync(CreateBusinessGroupRequest request, CancellationToken cancellationToken = default)
    {
        return PostAsJsonAsync<CreateBusinessGroupRequest, OrganizationEntityResponse>("api/organization/business-groups", request, cancellationToken);
    }

    public Task<OrganizationEntityResponse> CreateCompanyAsync(CreateCompanyRequest request, CancellationToken cancellationToken = default)
    {
        return PostAsJsonAsync<CreateCompanyRequest, OrganizationEntityResponse>("api/organization/companies", request, cancellationToken);
    }

    public Task<OrganizationEntityResponse> CreateBusinessUnitAsync(CreateBusinessUnitRequest request, CancellationToken cancellationToken = default)
    {
        return PostAsJsonAsync<CreateBusinessUnitRequest, OrganizationEntityResponse>("api/organization/business-units", request, cancellationToken);
    }

    public Task<HumanResourcesCatalogsResponse> GetCatalogsAsync(CancellationToken cancellationToken = default)
    {
        return GetRequiredFromJsonAsync<HumanResourcesCatalogsResponse>("api/hr/catalogs", cancellationToken);
    }

    public Task<IReadOnlyList<PersonResponse>> GetPeopleAsync(CancellationToken cancellationToken = default)
    {
        return GetRequiredFromJsonAsync<IReadOnlyList<PersonResponse>>("api/people", cancellationToken);
    }

    public Task<PersonResponse> CreatePersonAsync(CreatePersonRequest request, CancellationToken cancellationToken = default)
    {
        return PostAsJsonAsync<CreatePersonRequest, PersonResponse>("api/people", request, cancellationToken);
    }

    public Task<IReadOnlyList<StaffingPositionResponse>> GetStaffingPositionsAsync(CancellationToken cancellationToken = default)
    {
        return GetRequiredFromJsonAsync<IReadOnlyList<StaffingPositionResponse>>("api/staffing/positions", cancellationToken);
    }

    public Task<StaffingPositionResponse> CreateStaffingPositionAsync(CreateStaffingPositionRequest request, CancellationToken cancellationToken = default)
    {
        return PostAsJsonAsync<CreateStaffingPositionRequest, StaffingPositionResponse>("api/staffing/positions", request, cancellationToken);
    }

    public Task<IReadOnlyList<JobTemplateResponse>> GetJobTemplatesAsync(CancellationToken cancellationToken = default)
    {
        return GetRequiredFromJsonAsync<IReadOnlyList<JobTemplateResponse>>("api/staffing/job-templates", cancellationToken);
    }

    public Task<JobTemplateResponse> CreateJobTemplateAsync(CreateJobTemplateRequest request, CancellationToken cancellationToken = default)
    {
        return PostAsJsonAsync<CreateJobTemplateRequest, JobTemplateResponse>("api/staffing/job-templates", request, cancellationToken);
    }

    public Task<IReadOnlyList<JobTemplatePositionResponse>> GetJobTemplatePositionsAsync(CancellationToken cancellationToken = default)
    {
        return GetRequiredFromJsonAsync<IReadOnlyList<JobTemplatePositionResponse>>("api/staffing/job-template-positions", cancellationToken);
    }

    public Task<JobTemplatePositionResponse> AddJobTemplatePositionAsync(
        Guid jobTemplateId,
        AddJobTemplatePositionRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostAsJsonAsync<AddJobTemplatePositionRequest, JobTemplatePositionResponse>(
            $"api/staffing/job-templates/{jobTemplateId}/positions",
            request,
            cancellationToken);
    }

    public Task<IReadOnlyList<EmploymentResponse>> GetEmploymentsAsync(CancellationToken cancellationToken = default)
    {
        return GetRequiredFromJsonAsync<IReadOnlyList<EmploymentResponse>>("api/hiring", cancellationToken);
    }

    public Task<EmploymentResponse> HirePersonAsync(HirePersonRequest request, CancellationToken cancellationToken = default)
    {
        return PostAsJsonAsync<HirePersonRequest, EmploymentResponse>("api/hiring", request, cancellationToken);
    }

    private async Task<T> GetRequiredFromJsonAsync<T>(string requestUri, CancellationToken cancellationToken)
    {
        return await GetFromJsonAsync<T>(requestUri, cancellationToken)
            ?? throw new InvalidOperationException("El API no devolvio datos.");
    }

    private async Task<TResponse> PostAsJsonAsync<TRequest, TResponse>(
        string requestUri,
        TRequest payload,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = JsonContent.Create(payload)
        };

        var response = await SendAsync(request, cancellationToken);
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken)
            ?? throw new InvalidOperationException("El API no devolvio datos.");
    }

    private async Task<T?> GetFromJsonAsync<T>(string requestUri, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        var response = await SendAsync(request, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
    }

    private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await GetAccessTokenAsync();

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        var response = await httpClient.SendAsync(request, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return response;
        }

        var error = await response.Content.ReadFromJsonAsync<ApiError>(cancellationToken);
        throw new InvalidOperationException(error?.Message ?? "El API rechazo la operacion.");
    }

    private async Task<string?> GetAccessTokenAsync()
    {
        var state = await authenticationStateProvider.GetAuthenticationStateAsync();
        return state.User.FindFirst("access_token")?.Value;
    }

    private sealed record ApiError(string Message);
}
