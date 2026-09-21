using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Processor.Models;

public class StationStatusHistory
{
	[BsonId]
	[BsonRepresentation(BsonType.ObjectId)]
	public string? Id { get; set; }
	public string StationId { get; set; } = null!;
	public int NumBikesAvailable { get; set; }
	public int NumBikesDisabled { get; set; }
	public int NumDocksAvailable { get; set; }
	public int NumDocksDisabled { get; set; }
	public int NumEbikesAvailable { get; set; }
	public int? NumScootersAvailable { get; set; }
	public int? NumScootersUnavailable { get; set; }
	public bool IsInstalled { get; set; }
    public bool IsRenting { get; set; }
    public bool IsReturning { get; set; }
	public List<VehicleTypeAvailability>
		VehicleTypesAvailable { get; set; } = new();
	public long LastReported { get; set; }
	public DateTime RecordedAt { get; set; }
}