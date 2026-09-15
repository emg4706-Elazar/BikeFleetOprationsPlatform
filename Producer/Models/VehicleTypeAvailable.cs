using System.Text.Json.Serialization;

namespace Producer.Models;

public class VehicleTypeAvailable
{
    [JsonPropertyName("vehicle_type_id")]
    public int VehiclTypeId { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }
}