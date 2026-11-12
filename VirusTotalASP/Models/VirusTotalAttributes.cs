namespace VirusTotalASP.Models;

public class VirusTotalAttributes
{
    public VirusTotalStats? Stats { get; set; }
    public Dictionary<string, VirusTotalScanResult>? Results { get; set; }
    public string? Status { get; set; }
}

