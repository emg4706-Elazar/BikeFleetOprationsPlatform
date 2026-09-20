

namespace Api.Models;

public class StationInformationResponseDto
{
    public string StationId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? ShortName { get; set; }
    public decimal Longitude { get; set; }
    public decimal Latitude { get; set; }
    public string? RegionId { get; set; }
    public int Capacity { get; set; }
    public string? AndroidUri { get; set; }
    public string? IosUri { get; set; }
    public string? WebUri { get; set; }
}
