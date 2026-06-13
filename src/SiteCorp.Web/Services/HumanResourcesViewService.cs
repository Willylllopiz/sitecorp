using System.Text.Json;
using SiteCorp.Shared;
using SiteCorp.Web.Clients;

namespace SiteCorp.Web.Services;

public sealed class HumanResourcesViewService(
    HumanResourcesApiClient apiClient,
    SampleHumanResourcesData sampleData)
{
    public async Task<HumanResourcesSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await apiClient.GetSnapshotAsync(cancellationToken);
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException or JsonException or InvalidOperationException)
        {
            return sampleData.CreateSnapshot(isLive: false);
        }
    }
}

