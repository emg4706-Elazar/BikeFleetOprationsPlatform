using Api.Models;
using StackExchange.Redis;
using System.Text.Json;


namespace Api.Repositories;

public class StationStatusRepository :
    IStationStatusRepository
{
    private readonly IDatabase _database;

    public StationStatusRepository(
        IDatabase database)
    {
        _database = database;
    }

    public async Task<StationStatusDto?>
        GetByStationIdAsync(
        string stationId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string redisKey =
            $"station-status:{stationId}";

        RedisValue value =
            await _database.StringGetAsync(redisKey);


        if (value.IsNullOrEmpty)
        {
            return null;
        }

        return JsonSerializer.Deserialize<
            StationStatusDto>(
            value.ToString());
    }
}
