using Microsoft.AspNetCore.Mvc;
using VirusTotalASP.Models;
using VirusTotalASP.Services;

namespace VirusTotalASP.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScanController : ControllerBase
{
    private readonly IVirusTotalService virusTotalService;
    private readonly ILogger<ScanController> logger;

    public ScanController(IVirusTotalService virusTotalService, ILogger<ScanController> logger)
    {
        this.virusTotalService = virusTotalService;
        this.logger = logger;
    }

    [HttpPost("file")]
    [RequestSizeLimit(32_000_000)] // Лимит размера файла: 32MB
    public async Task<ActionResult<ScanFileResponse>> ScanFile([FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new ScanFileResponse
            {
                Status = "error",
                Message = "Файл не предоставлен"
            });
        }

        if (file.Length > 32_000_000) // 32MB - максимальный размер файла
        {
            return BadRequest(new ScanFileResponse
            {
                Status = "error",
                Message = "Размер файла превышает лимит 32MB"
            });
        }

        try
        {
            var result = await virusTotalService.ScanFileAsync(file, cancellationToken);
            
            if (result.Status == "error")
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при обработке запроса на сканирование файла");
            return StatusCode(500, new ScanFileResponse
            {
                Status = "error",
                Message = $"Внутренняя ошибка сервера: {ex.Message}"
            });
        }
    }

    [HttpGet("result/{id}")]
    public async Task<ActionResult<ScanResultResponse>> GetResult(string id, [FromQuery] string? type = "analysis", CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest(new ScanResultResponse
            {
                Status = "error",
                Message = "Требуется Analysis ID или File ID"
            });
        }

        try
        {
            ScanResultResponse result;
            
            if (type == "file")
            {
                result = await virusTotalService.GetFileResultAsync(id, cancellationToken);
            }
            else
            {
                result = await virusTotalService.GetAnalysisResultAsync(id, cancellationToken);
            }

            if (result.Status == "error" && !string.IsNullOrEmpty(result.Message))
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении результата сканирования");
            return StatusCode(500, new ScanResultResponse
            {
                Status = "error",
                Message = $"Внутренняя ошибка сервера: {ex.Message}"
            });
        }
    }
}

