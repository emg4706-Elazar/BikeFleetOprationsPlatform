using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Processor.Models;

public class StationStatusHistory
{
	[BsonId]
	[BsonRepresentation(BsonType.ObjectId)]
	public string? Id { get; set; }
	public string StationId { get; set; } = null!;
	public Availability Availability { get; set; } = null!;
	public OperationalState OperationalState { get; set; } = null!;
	public List<VehicleTypeAvailability> VehicleTypesAvailable { get; set; }
		= new();
	public long LastReported { get; set; }
	public DateTime RecordedAt { get; set; }
}