using Microsoft.AspNetCore.Mvc;
using NavJobsProxyService.Services;

namespace NavJobsProxyService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NavController : ControllerBase
{
    private readonly INavService _navService;
    private readonly ILogger<NavController> _logger;

    public NavController(INavService navService, ILogger<NavController> logger)
    {
        _navService = navService;
        _logger = logger;
    }

    [HttpPost("helloworld")]
    public async Task<IActionResult> HelloWorld([FromBody] HelloWorldRequest request)
    {
        try
        {
            _logger.LogInformation("Received HelloWorld request with input: {input}", request.InputText);
            var result = await _navService.HelloWorldAsync(request.InputText);
            return Ok(new HelloWorldResponse { Result = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling NAV HelloWorld");
            return StatusCode(500, new { Error = "Failed to call NAV service", Details = ex.Message });
        }
    }
    [HttpPost("startjob")]
    public async Task<IActionResult> StartJob([FromBody] StartJobRequest request)
    {
        try
        {
            _logger.LogInformation("Received StartJob request with jobId: {jobId}", request.JobId);
            var result = await _navService.StartJobAsync(request.JobId, request.InputJson);
            return Ok(new { Result = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling NAV StartJob");
            return StatusCode(500, new { Error = "Failed to call NAV service", Details = ex.Message });
        }
    }
    [HttpGet("checkjob/{jobId}")]
    public async Task<IActionResult> CheckJob(string jobId)
    {
        try
        {
            _logger.LogInformation("Received CheckJob request with jobId: {jobId}", jobId);
            var result = await _navService.CheckJobAsync(jobId);
            return Ok(new CheckJobResponse { Result = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling NAV CheckJob");
            return StatusCode(500, new { Error = "Failed to call NAV service", Details = ex.Message });
        }
    }
}

public class HelloWorldRequest
{
    public string InputText { get; set; } = string.Empty;
}

public class HelloWorldResponse
{
    public string Result { get; set; } = string.Empty;
}

public class StartJobRequest
{
    public string JobId { get; set; } = string.Empty;
    public string InputJson { get; set; } = string.Empty;
}


public class StartJobResponse
{
    public string Result { get; set; } = string.Empty;
}

public class CheckJobResponse
{
    public string Result { get; set; } = string.Empty;
}
