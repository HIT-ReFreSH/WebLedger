using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace HitRefresh.WebLedger.Web.Services.HealthChecks;

public class ServiceAvailabilityHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;

    public ServiceAvailabilityHealthCheck(IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var url = _configuration["Monitoring:ServiceUrl"];
        if (string.IsNullOrEmpty(url))
        {
            // No external service configured; treat as healthy
            return HealthCheckResult.Healthy("No service URL configured");
        }

        try
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync(url, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("Service reachable");
            }
            return HealthCheckResult.Unhealthy($"Service returned status {(int)response.StatusCode} {response.StatusCode}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Service check exception: " + ex.Message, ex);
        }
    }
}
