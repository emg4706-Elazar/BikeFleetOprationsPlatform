

namespace Producer.Models;

public class StationInformationResponseDto
{
    public StationInformationDataDto Data { get; set; } = null!;
    public int LastUpdated { get; set; }
    public int Ttl { get; set; }
    public string Version { get; set; } = null!;
}
