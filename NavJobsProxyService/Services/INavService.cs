namespace NavJobsProxyService.Services;

public interface INavService
{
    Task<string> HelloWorldAsync(string inputText);
    Task<string> StartJobAsync(string jobId);

}
