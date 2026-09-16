using Microsoft.Extensions.Logging;
using Processor.Models;
using System.Text.Json;

namespace Processor.Handlers;

public class StationStatusHandler : IStationStatusHandler
{
    private readonly ILogger<StationStatusHandler> _logger;
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
    
    public StationStatusHandler(
        ILogger<StationStatusHandler> logger)
    {
        _logger = logger;
    }


    public Task HandleAsync(
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

        _logger.LogInformation(
            "Handled station status for station {StionId}",
            station.StationId);

        return Task.CompletedTask;
    }
}
