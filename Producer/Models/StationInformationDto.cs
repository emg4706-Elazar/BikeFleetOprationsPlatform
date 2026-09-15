

namespace Producer.Models;

public class StationInformationDto
{
    public string StationId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? ShortName { get; set; }
    public double Lon { get; set; }
    public double Lat { get; set; }
    public string? RegionId { get; set; }
    public int Capacity { get; set; }
    public RentalUrisDto? RentalUris { get; set; } = null!;
}
