namespace NavJobsProxyService.Services;

public class NavServiceOptions
{
    public Dictionary<string, string> Companies { get; set; } = new();
    public int TimeoutMinutes { get; set; } = 60;
}
