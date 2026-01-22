using System.ServiceModel;
using Microsoft.Extensions.Logging;
using ServiceReference;

namespace NavJobsProxyService.Services;

public class NavService : INavService
{
    private readonly ILogger<NavService> _logger;
    private readonly BasicHttpBinding _binding;
    private readonly EndpointAddress _endpoint;

    public NavService(ILogger<NavService> logger)
    {
        _logger = logger;

        _binding = new BasicHttpBinding
        {
            SendTimeout = TimeSpan.FromMinutes(1),
            ReceiveTimeout = TimeSpan.FromMinutes(1),
            Security =
            {
                Mode = BasicHttpSecurityMode.TransportCredentialOnly,
                Transport = { ClientCredentialType = HttpClientCredentialType.Windows }
            }
        };

        _endpoint = new EndpointAddress("http://localhost:7649/DynamicsNAVCarloTEST/WS/MOTORFORUM DRAMMEN/Codeunit/TestNavWs?wsdl");
    }

    public async Task<string> HelloWorldAsync(string inputText)
    {
        var client = new TestNavWs_PortClient(_binding, _endpoint);

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
    public async Task<string> StartJobAsync(string jobId)
    {
        var client = new TestNavWs_PortClient(_binding, _endpoint);
        try
        {
            client.ClientCredentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials;
            client.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            var result = await client.StartJobAsync(jobId);
            return result.return_value;
        }
        finally
        {
            try { await client.CloseAsync(); }
            catch { client.Abort(); }
        }
    }
}
