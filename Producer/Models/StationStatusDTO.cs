using System.Text.Json.Serialization;

namespace Producer.Models;

public class StationStatusDto
{
	[JsonPropertyName("station_id")]
	public string StationId { get; set; } = null!;

	[JsonPropertyName("num_bikes_available")]
	public int NumBikesAvailable { get; set; }

	[JsonPropertyName("num_bikes_disabled")]
	public int NumBikesDisabled { get; set; }

	[JsonPropertyName("num_docks_available")]
	public int NumDocksAvailable { get; set; }

	[JsonPropertyName("num_docks_disabled")]
	public int NumDocksDisabled { get; set; }

	[JsonPropertyName("is_installed")]
	public int IsInstalled { get; set; }

	[JsonPropertyName("is_renting")]
	public int IsRenting { get; set; }

	[JsonPropertyName("is_returning")]
	public int IsReturning { get; set; }

	[JsonPropertyName("last_reported")]
	public long LastReported { get; set; }

	[JsonPropertyName("vehicle_types_available")]
	public List<VehicleTypeAvailable> VehicleTypesAvailable { get; set; }
        = new();

    [JsonPropertyName("num_ebikes_available")]
    public int NumEbikesAvailable { get; set; }

	[JsonPropertyName("num_scooters_available")]
    public int? NumScootersAvailable { get; set; }

	[JsonPropertyName("num_scooters_unavailable")]
    public int? NumScootersUnavailable { get; set; }
}