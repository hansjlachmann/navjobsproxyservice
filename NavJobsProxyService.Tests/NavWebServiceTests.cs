using System.ServiceModel;
using ServiceReference;
using Xunit;

namespace NavJobsProxyService.Tests;

public class NavWebServiceTests
{
    private const string NavServiceUrl = "http://localhost:7649/DynamicsNAVCarloTEST/WS/MOTORFORUM%20DRAMMEN/Codeunit/TestNavWs";

    [Fact]
    public async Task HelloWorld_ReturnsValidResponse()
    {
        // Arrange
        var binding = new BasicHttpBinding
        {
            MaxBufferSize = int.MaxValue,
            MaxReceivedMessageSize = int.MaxValue,
            SendTimeout = TimeSpan.FromMinutes(1),
            ReceiveTimeout = TimeSpan.FromMinutes(1),
            Security =
            {
                Mode = BasicHttpSecurityMode.TransportCredentialOnly,
                Transport = { ClientCredentialType = HttpClientCredentialType.Windows }
            }
        };

        var endpoint = new EndpointAddress(NavServiceUrl);
        var client = new TestNavWs_PortClient(binding, endpoint);
        client.ClientCredentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials;
        client.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

        try
        {
            // Act
            var response = await client.HelloWorldAsync("Test from automated test");

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.return_value);
            Assert.Contains("Test from automated test", response.return_value);

            // Output for visibility
            Console.WriteLine($"NAV Response: {response.return_value}");
        }
        finally
        {
            await client.CloseAsync();
        }
    }

    [Fact]
    public async Task NavService_IsReachable()
    {
        // Arrange
        var binding = new BasicHttpBinding
        {
            MaxBufferSize = int.MaxValue,
            MaxReceivedMessageSize = int.MaxValue,
            SendTimeout = TimeSpan.FromSeconds(10),
            ReceiveTimeout = TimeSpan.FromSeconds(10),
            Security =
            {
                Mode = BasicHttpSecurityMode.TransportCredentialOnly,
                Transport = { ClientCredentialType = HttpClientCredentialType.Windows }
            }
        };

        var endpoint = new EndpointAddress(NavServiceUrl);
        var client = new TestNavWs_PortClient(binding, endpoint);
        client.ClientCredentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials;
        client.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

        Exception? caughtException = null;

        try
        {
            // Act - just try to call the service
            var response = await client.HelloWorldAsync("Connection test");

            // If we get here, the service is reachable
            Assert.True(true, "NAV service is reachable");
            Console.WriteLine("NAV service connection: SUCCESS");
        }
        catch (Exception ex)
        {
            caughtException = ex;
            Console.WriteLine($"NAV service connection: FAILED - {ex.Message}");
        }
        finally
        {
            try { await client.CloseAsync(); } catch { client.Abort(); }
        }

        // Fail the test if there was an exception
        Assert.Null(caughtException);
    }
}
