using System.ServiceModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceReference;

namespace NavJobsProxyService.Services;

public class NavService : INavService
{
    private readonly ILogger<NavService> _logger;
    private readonly BasicHttpBinding _binding;
    private readonly Dictionary<string, string> _companies;

    public NavService(ILogger<NavService> logger, IOptions<NavServiceOptions> options)
    {
        _logger = logger;
        _companies = options.Value.Companies;
        var timeoutMinutes = options.Value.TimeoutMinutes;

        _binding = new BasicHttpBinding
        {
            SendTimeout = TimeSpan.FromMinutes(timeoutMinutes),
            ReceiveTimeout = TimeSpan.FromMinutes(timeoutMinutes),
            Security =
            {
                Mode = BasicHttpSecurityMode.TransportCredentialOnly,
                Transport = { ClientCredentialType = HttpClientCredentialType.Windows }
            }
        };

        _logger.LogInformation("NavService initialized with timeout: {timeout} minutes", timeoutMinutes);
    }

    private EndpointAddress GetEndpoint(string companyName)
    {
        if (!_companies.TryGetValue(companyName, out var url))
        {
            throw new ArgumentException($"Company '{companyName}' not found in configuration");
        }
        return new EndpointAddress(url);
    }

    public async Task<string> HelloWorldAsync(string inputText)
    {
        var endpoint = GetEndpoint("MOTORFORUM DRAMMEN");
        var client = new TestNavWs_PortClient(_binding, endpoint);

        try
        {
            client.ClientCredentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials;
            client.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            var result = await client.HelloWorldAsync(inputText);
            return result.return_value;
        }
        finally
        {
            try
            {
                await client.CloseAsync();
            }
            catch
            {
                client.Abort();
            }
        }
    }
    public async Task<string> StartJobAsync(string jobId, string companyName, string inputJson)
    {
        var endpoint = GetEndpoint(companyName);
        _logger.LogInformation("Calling NAV StartJob at {url} with jobId: {jobId}", endpoint.Uri, jobId);

        var client = new TestNavWs_PortClient(_binding, endpoint);
        try
        {
            client.ClientCredentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials;
            client.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            var result = await client.StartJobAsync(jobId, inputJson);
            return result.return_value;
        }
        finally
        {
            try { await client.CloseAsync(); }
            catch { client.Abort(); }
        }
    }
    public async Task<string> CheckJobAsync(string jobId, string companyName)
    {
        var endpoint = GetEndpoint(companyName);
        _logger.LogInformation("Calling NAV CheckJob at {url} with jobId: {jobId}", endpoint.Uri, jobId);

        var client = new TestNavWs_PortClient(_binding, endpoint);
        try
        {
            client.ClientCredentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials;
            client.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

            var result = await client.CheckJobAsync(jobId);
            _logger.LogInformation("NAV CheckJob response: {response}", result.return_value);

            return result.return_value;
        }
        finally
        {
            try { await client.CloseAsync(); }
            catch { client.Abort(); }
        }
    }
}
