namespace VirusTotalASP.Models;

public class ScanResultResponse
{
    public string? Status { get; set; }
    public VirusTotalStats? Stats { get; set; }
    public Dictionary<string, VirusTotalScanResult>? Results { get; set; }
    public string? Message { get; set; }
}

