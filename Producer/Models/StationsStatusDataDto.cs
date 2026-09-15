using System.Text.Json.Serialization;

namespace Producer.Models;

public class StationsStatusDataDto
{
    [JsonPropertyName("stations")]
    public List<StationStatusDto> Stations { get; set; } = new();
}