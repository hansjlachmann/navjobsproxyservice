using Microsoft.AspNetCore.Mvc;
using NavJobsProxyService.Services;
using File = System.IO.File;


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


    [HttpGet("job/{jobId}/pdf")]
    public async Task<IActionResult> GetJobPdf(string jobId)
    {
        try
        {
            var pdfPath = $@"C:\ProgramData\Microsoft\Microsoft Dynamics NAV\60\Server\MicrosoftDynamicsNavServer$CarloTEST\users\MOTORDATA\hjl_admin\TEMP\{jobId}-CustomerBalanceToDate.pdf";
            
            _logger.LogInformation("Getting PDF for job {jobId} from {path}", jobId, pdfPath);
            
            if (!System.IO.File.Exists(pdfPath))
            {
                return NotFound(new { Error = "PDF not found", Path = pdfPath });
            }
            
            // Wait for file to be available (not locked)
            var maxRetries = 10;
            var delayMs = 500;
            
            for (int i = 0; i < maxRetries; i++)
            {
                if (IsFileReady(pdfPath))
                {
                    var pdfBytes = await System.IO.File.ReadAllBytesAsync(pdfPath);
                    var base64 = Convert.ToBase64String(pdfBytes);
                    
                    return Ok(new GetJobPdfResponse 
                    { 
                        JobId = jobId,
                        FileName = Path.GetFileName(pdfPath),
                        Base64 = base64 
                    });
                }
                
                _logger.LogInformation("PDF file is locked, retry {retry}/{maxRetries}", i + 1, maxRetries);
                await Task.Delay(delayMs);
            }
            
            return StatusCode(503, new { Error = "PDF file is locked", Message = "File is still being generated, try again later" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting PDF for job {jobId}", jobId);
            return StatusCode(500, new { Error = "Failed to get PDF", Details = ex.Message });
        }
    }
    
    private bool IsFileReady(string filePath)
    {
        try
        {
            using var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            return true;
        }
        catch (IOException)
        {
            return false;
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

public class GetJobPdfResponse
{
    public string JobId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Base64 { get; set; } = string.Empty;
}

