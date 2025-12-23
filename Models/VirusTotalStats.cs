namespace VirusTotalASP.Models;

public class VirusTotalStats
{
    public int? Harmless { get; set; }
    public int? TypeUnsupported { get; set; }
    public int? Suspicious { get; set; }
    public int? ConfirmedTimeout { get; set; }
    public int? Timeout { get; set; }
    public int? Failure { get; set; }
    public int? Malicious { get; set; }
    public int? Undetected { get; set; }
}

