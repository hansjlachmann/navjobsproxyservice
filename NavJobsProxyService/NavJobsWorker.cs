using System;
using System.ServiceModel;               // For BasicHttpBinding, EndpointAddress
using System.ServiceModel.Channels;      // Optional, for more binding options
using System.ServiceModel.Security;      // Optional, for client credential security
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceReference;                   // Your generated NAV WCF reference

public class NavJobsWorker : BackgroundService
{
    private readonly ILogger<NavJobsWorker> _logger;

    public NavJobsWorker(ILogger<NavJobsWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("NAV Jobs Worker started at: {time}", DateTimeOffset.Now);

        var binding = new BasicHttpBinding
        {
            SendTimeout = TimeSpan.FromMinutes(1),
            ReceiveTimeout = TimeSpan.FromMinutes(1),
            Security =
            {
                Mode = BasicHttpSecurityMode.TransportCredentialOnly,
                Transport = { ClientCredentialType = HttpClientCredentialType.Windows }
            }
        };

        var endpoint = new EndpointAddress("http://localhost:7649/DynamicsNAVCarloTEST/WS/MOTORFORUM DRAMMEN/Codeunit/TestNavWs?wsdl");

        var client = new TestNavWs_PortClient(binding, endpoint);

        // Use default credentials or explicit
        client.ClientCredentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials;
        client.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = await client.HelloWorldAsync("Test from Worker");
                _logger.LogInformation("NAV Service responded: {response}", result.return_value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling NAV service");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
