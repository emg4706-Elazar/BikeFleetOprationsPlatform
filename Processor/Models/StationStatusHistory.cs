using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Processor.Models;

public class StationStatusHistory
{
	[BsonId]
	[BsonRepresentation(BsonType.ObjectId)]
	public string? Id { get; set; }
	public string StationId { get; set; }
	public Availability Availability{ get; set; }
	public OprationalState OperationalState { get; set; }
	public List<VehicleTypeAvailability> VehicleTypesAvailable { get; set; }
		= new();
	public long LastReported { get; set; }
	public DateTime RecordedAt { get; set; }
}