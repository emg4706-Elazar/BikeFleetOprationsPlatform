

namespace Processor.Models;

public class VehicleTypeDto
{
    public string VehicleTypeId { get; set; } = null!;
    public string FormFactor { get; set; } = null!;
    public string PropulsionType { get; set; } = null!;
    public double? MaxRangeMeters { get; set; }
}
