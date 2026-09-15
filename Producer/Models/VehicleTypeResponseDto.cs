
namespace Producer.Models;

public class VehicleTypeResponseDto
{
    public VehicleTypeDataDto Data { get; set; } = null!;
    public long LastUpdated { get; set; }
    public int Ttl { get; set; }
    public string Version { get; set; } = null!;
}
