using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Producer.Models;

public class StationStatusHistoty
{
	[BsonId]
	[BsonRepresentation(BsonType.ObjectId)]
	public string? Id { get; set; }
	public string stationId { get; set; }
	public Availability{ get; set; }
	public OprationalState OperationalState { get; set; }
	public List<VehicleTypeAvailability> VehicleTypesAvailavle { get; set; }
		= new();
	public long LastReported { get; set; }
	public DateTime RecordedAt { get; set; }
}