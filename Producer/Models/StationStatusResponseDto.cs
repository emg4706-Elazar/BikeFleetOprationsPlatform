using System.Text.Json.Serialization;

namespace Producer.Models;

public class StationStatusResponseDto
{
	[JsonPropertyName("data")]
	public StationStatusDataDto Data { get; set; } = null!;

	[JsonPropertyName("last_updated")]
	public long LastUpdated { get; set; }

	[JsonPropertyName("ttl")]
	public int Ttl { get; set; }

	[JsonPropertyName("version")]
	public string Version { get; set; } = null!;
}