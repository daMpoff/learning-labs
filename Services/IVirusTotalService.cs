using VirusTotalASP.Models;

namespace VirusTotalASP.Services;

public interface IVirusTotalService
{
    Task<ScanFileResponse> ScanFileAsync(IFormFile file, CancellationToken cancellationToken = default);
    Task<ScanResultResponse> GetAnalysisResultAsync(string analysisId, CancellationToken cancellationToken = default);
    Task<ScanResultResponse> GetFileResultAsync(string fileId, CancellationToken cancellationToken = default);
}

