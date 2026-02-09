using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi_task.Applications.Services;
using webapi_task.Infrastructure;

namespace webapi_task.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IFileProcessingService _fileProcessingService;
    private readonly IFileQueryService _fileQueryService;

    public FilesController(IFileProcessingService fileProcessingService,
        IFileQueryService fileQueryService)
    {
        _fileProcessingService = fileProcessingService;
        _fileQueryService = fileQueryService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        try
        {
            // Проверка наличия файла
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Error = "File is empty" });
            }

            // Проверка расширения файла
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".csv")
            {
                return BadRequest(new { Error = "Only CSV files are allowed" });
            }

            // Обработка файла
            var result = await _fileProcessingService.ProcessFileAsync(file.OpenReadStream(), file.FileName);

            if (!result.IsSuccess)
            {
                return BadRequest(new { Errors = result.Errors });
            }

            return Ok(new
            {
                Message = "File processed successfully",
                ResultId = result.ResultId,
                FileName = file.FileName
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "An error occurred while processing the file", Details = ex.Message });
        }
    }

    [HttpGet("results")]
    public async Task<IActionResult> GetFilteredResults(
        [FromQuery] string? fileName = null,
        [FromQuery] DateTime? startDateFrom = null,
        [FromQuery] DateTime? startDateTo = null,
        [FromQuery] double? averageValueFrom = null,
        [FromQuery] double? averageValueTo = null,
        [FromQuery] double? averageExecutionTimeFrom = null,
        [FromQuery] double? averageExecutionTimeTo = null)
    {
        try
        {
            var results = await _fileQueryService.GetFilteredResultsAsync(
                fileName,
                startDateFrom,
                startDateTo,
                averageValueFrom,
                averageValueTo,
                averageExecutionTimeFrom,
                averageExecutionTimeTo);

            return Ok(results);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "An error occurred while fetching results", Details = ex.Message });
        }
    }

    [HttpGet("{fileName}/last-values")]
    public async Task<IActionResult> GetLastValues(string fileName)  // Убрали [FromQuery] int count = 10
    {
        try
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return BadRequest(new { Error = "File name is required" });
            }

            var values = await _fileQueryService.GetLastValuesAsync(fileName);

            if (!values.Any())
            {
                return NotFound(new { Error = $"No data found for file: {fileName}" });
            }

            return Ok(values);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "An error occurred while fetching values", Details = ex.Message });
        }
    }
}