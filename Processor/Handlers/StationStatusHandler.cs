using Microsoft.Extensions.Logging;
using Processor.Models;
using System.Text.Json;
using MongoDB.Driver;
using StackExchange.Redis;

namespace Processor.Handlers;

public class StationStatusHandler : IStationStatusHandler
{
    private readonly ILogger<StationStatusHandler> _logger;
    private readonly IDatabase _redisDatabase;
    private readonly IMongoCollection<StationStatusHistory> _collection;
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

    public StationStatusHandler(
        IDatabase redisDatabase,
        IMongoCollection<StationStatusHistory> collection,
        ILogger<StationStatusHandler> logger)
    {
        _logger = logger;
        _redisDatabase = redisDatabase;
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

        string redisKey =
            $"station-status:{station.StationId}";

        await _redisDatabase.StringSetAsync(
            redisKey,
            json);

        StationStatusHistory history =
            MapToHistory(station);

        await _collection.InsertOneAsync(
            history,
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "Saved station history for station {StationId}.",
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
