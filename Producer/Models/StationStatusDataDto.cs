using System.Text.Json.Serialization;

namespace Producer.Models;

public class StationStatusDataDto
{
    [JsonPropertyName("stations")]
    public List<StationStatusDto> Stations { get; set; } = new();
}