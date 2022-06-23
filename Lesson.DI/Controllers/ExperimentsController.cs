using Microsoft.AspNetCore.Mvc;

namespace Lesson.DI.Controllers;

[ApiController]
[Route("Experiments")]
public class ExperimentsController : ControllerBase
{
    private readonly ILogger<ExperimentsController> _logger;
    private readonly CancellationToken _cancellationToken;

    public ExperimentsController(ILogger<ExperimentsController> logger, 
        CancellationToken cancellationToken)
    {
        _logger = logger;
        _cancellationToken = cancellationToken;
    }

    [HttpGet("ForeverPrint")]
    public async Task ForeverPrint(
        [FromQuery] string text)
    {
        while (true)
        {
            _logger.LogInformation("{Text} DateTime: {DateTime}", text, DateTime.Now);
            await Task.Delay(TimeSpan.FromSeconds(1), _cancellationToken);
            _cancellationToken.ThrowIfCancellationRequested();
        }
    }
}