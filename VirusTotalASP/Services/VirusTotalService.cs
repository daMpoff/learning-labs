using System.Net.Http.Headers;
using System.Text.Json;
using VirusTotalASP.Models;

namespace VirusTotalASP.Services;

public class VirusTotalService : IVirusTotalService
{
    private readonly HttpClient httpClient;
    private readonly ILogger<VirusTotalService> logger;
    private readonly string apiKey;
    private const string BaseUrl = "https://www.virustotal.com/api/v3";

    public VirusTotalService(HttpClient httpClient, IConfiguration configuration, ILogger<VirusTotalService> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
        var configuredKey = configuration["VirusTotal:ApiKey"];
        if (string.IsNullOrWhiteSpace(configuredKey))
        {
            throw new InvalidOperationException("Установите переменную окружения VirusTotal__ApiKey.");
        }
        apiKey = configuredKey;

        this.httpClient.BaseAddress = new Uri(BaseUrl + "/");
        this.httpClient.DefaultRequestHeaders.Add("x-apikey", apiKey);
    }

    public async Task<ScanFileResponse> ScanFileAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();
            using var streamContent = new StreamContent(fileStream);
            
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
            content.Add(streamContent, "file", file.FileName);

            var requestUrl = new Uri(httpClient.BaseAddress!, "files");
            logger.LogInformation("Загрузка файла в VirusTotal: {Url}", requestUrl);
            
            var response = await httpClient.PostAsync("files", content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var uploadResponse = JsonSerializer.Deserialize<VirusTotalFileUploadResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (uploadResponse?.Data?.Id != null)
                {
                    // Получаем analysis_id из заголовка Location
                    // Формат: https://www.virustotal.com/api/v3/analyses/{analysis_id}
                    var locationHeader = response.Headers.Location?.ToString();
                    var analysisId = locationHeader?.Split('/').LastOrDefault();
                    
                    // Если analysis_id не найден в Location, используем file_id для получения результатов
                    return new ScanFileResponse
                    {
                        FileId = uploadResponse.Data.Id,
                        AnalysisId = analysisId ?? uploadResponse.Data.Id, // Резервный вариант: используем file_id
                        Status = "queued",
                        Message = "Файл успешно загружен. Анализ поставлен в очередь."
                    };
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Ошибка VirusTotal API: {StatusCode} - {Content}", response.StatusCode, errorContent);

            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                return new ScanFileResponse
                {
                    Status = "error",
                    Message = "Превышен лимит запросов. Пожалуйста, попробуйте позже."
                };
            }

            return new ScanFileResponse
            {
                Status = "error",
                Message = $"Не удалось загрузить файл: {response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при сканировании файла");
            return new ScanFileResponse
            {
                Status = "error",
                Message = $"Ошибка: {ex.Message}"
            };
        }
    }

    public async Task<ScanResultResponse> GetAnalysisResultAsync(string analysisId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync($"analyses/{analysisId}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogInformation("Ответ анализа VirusTotal: {Content}", responseContent);
                
                var analysisResponse = JsonSerializer.Deserialize<VirusTotalAnalysisResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (analysisResponse?.Data?.Attributes != null)
                {
                    var result = new ScanResultResponse
                    {
                        Status = analysisResponse.Data.Attributes.Status ?? "unknown",
                        Stats = analysisResponse.Data.Attributes.Stats,
                        Results = analysisResponse.Data.Attributes.Results
                    };
                    
                    logger.LogInformation("Распарсенный результат - Статус: {Status}, ЕстьСтатистика: {HasStats}, ЕстьРезультаты: {HasResults}, КоличествоРезультатов: {Count}",
                        result.Status,
                        result.Stats != null,
                        result.Results != null,
                        result.Results?.Count ?? 0);
                    
                    return result;
                }
                else
                {
                    logger.LogWarning("Данные ответа анализа или атрибуты равны null. Ответ: {Response}", responseContent);
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Ошибка VirusTotal API: {StatusCode} - {Content}", response.StatusCode, errorContent);

            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                return new ScanResultResponse
                {
                    Status = "error",
                    Message = "Превышен лимит запросов. Пожалуйста, попробуйте позже."
                };
            }

            return new ScanResultResponse
            {
                Status = "error",
                Message = $"Не удалось получить результат анализа: {response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении результата анализа");
            return new ScanResultResponse
            {
                Status = "error",
                Message = $"Ошибка: {ex.Message}"
            };
        }
    }

    public async Task<ScanResultResponse> GetFileResultAsync(string fileId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync($"files/{fileId}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var fileResponse = JsonSerializer.Deserialize<VirusTotalAnalysisResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (fileResponse?.Data?.Attributes != null)
                {
                    return new ScanResultResponse
                    {
                        Status = "completed",
                        Stats = fileResponse.Data.Attributes.Stats,
                        Results = fileResponse.Data.Attributes.Results
                    };
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Ошибка VirusTotal API: {StatusCode} - {Content}", response.StatusCode, errorContent);

            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                return new ScanResultResponse
                {
                    Status = "error",
                    Message = "Превышен лимит запросов. Пожалуйста, попробуйте позже."
                };
            }

            // 404 означает, что файл не найден - возможно, еще не проиндексирован, возвращаем ошибку, но не логируем как критическую
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new ScanResultResponse
                {
                    Status = "error",
                    Message = "Файл не найден. Возможно, он еще не проиндексирован."
                };
            }

            return new ScanResultResponse
            {
                Status = "error",
                Message = $"Не удалось получить результат файла: {response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении результата файла");
            return new ScanResultResponse
            {
                Status = "error",
                Message = $"Ошибка: {ex.Message}"
            };
        }
    }
}
