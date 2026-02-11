namespace NavJobsProxyService.Services;

public interface INavService
{
    Task<string> HelloWorldAsync(string inputText);
    Task<string> StartJobAsync(string jobId, string companyName, string inputJson);
    Task<string> CheckJobAsync(string jobId, string companyName);
}
