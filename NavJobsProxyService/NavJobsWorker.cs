using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NavJobsProxyService.Services;

public class NavJobsWorker : BackgroundService
{
    private readonly ILogger<NavJobsWorker> _logger;
    private readonly INavService _navService;

    public NavJobsWorker(ILogger<NavJobsWorker> logger, INavService navService)
    {
        _logger = logger;
        _navService = navService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("NAV Jobs Worker started at: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = await _navService.HelloWorldAsync("Test from Worker");
                _logger.LogInformation("NAV Service responded: {response}", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling NAV service");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
