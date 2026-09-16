using System.Text.Json.Serialization;

namespace Processor.Models;

public class VehicleTypeAvailableDto
{
    [JsonPropertyName("vehicle_type_id")]
    public string VehicleTypeId { get; set; } = null!;

    [JsonPropertyName("count")]
    public int Count { get; set; }
}