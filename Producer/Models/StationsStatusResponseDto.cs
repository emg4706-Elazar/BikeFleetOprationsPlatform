using System.Text.Json.Serialization;

namespace Producer.Models;

public class StationsStatusResponseDto
{
	[JsonPropertyName("data")]
	public StationsStatusDataDto Data { get; set; } = null!;
}