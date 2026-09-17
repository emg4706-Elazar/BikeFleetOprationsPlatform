using Microsoft.Extensions.Logging;
using Processor.Models;
using System.Text.Json;
using MongoDB.Driver;

namespace Processor.Handlers;

public class StationStatusHandler : IStationStatusHandler
{
    private readonly ILogger<StationStatusHandler> _logger;
    private readonly IMongoCollection<StationStatusHistory> _collection;
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
    
    public StationStatusHandler(
        IMongoCollection<StationStatusHistory> collection,
        ILogger<StationStatusHandler> logger)
    {
        _logger = logger;
        _collection = collection;
    }


    public async Task HandleAsync(
        string json,
        CancellationToken cancellationToken)
    {
        StationStatusDto? station =
            JsonSerializer.Deserialize<
                StationStatusDto>(
                json,
                JsonOptions);

        if (station is null)
        {
            throw new JsonException(
                "Station status message returned null");
        }

        StationStatusHistory history =
            MapToHistory(station);

        await _collection.InsertOneAsync(
            history,
            cancellationToken: cancellationToken);

        _logger.LogDebug(
        "Saved status history for station {StationId}.",
        history.StationId);
    }
    
    private static StationStatusHistory MapToHistory(StationStatusDto dto)
    {
        return new StationStatusHistory
        {
            StationId = dto.StationId,
            NumBikesAvailable = dto.NumBikesAvailable,
            NumBikesDisabled = dto.NumBikesDisabled,
            NumDocksAvailable = dto.NumDocksAvailable,
            NumDocksDisabled = dto.NumDocksDisabled,
            NumEbikesAvailable = dto.NumEbikesAvailable,
            NumScootersAvailable = dto.NumScootersAvailable,
            NumScootersUnavailable = dto.NumScootersUnavailable,
            IsInstalled = dto.IsInstalled == 1,
            IsRenting = dto.IsRenting == 1,
            IsReturning = dto.IsReturning == 1,
            VehicleTypesAvailable =
                dto.VehicleTypesAvailable
                    .Select(vehicle =>
                        new VehicleTypeAvailability
                        {
                            VehicleTypeId = vehicle.VehicleTypeId,

                            Count = vehicle.Count
                        })
                        .ToList(),

            LastReported = dto.LastReported,
            RecordedAt = DateTime.UtcNow
        };
    }
}
