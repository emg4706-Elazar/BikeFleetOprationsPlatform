using Microsoft.Extensions.Logging;
using System.Text.Json;
using Processor.Models;

namespace Processor.Handlers;

public class StationInformationHandler :
    IStationInformationHandler
{
    private readonly ILogger<StationInformationHandler> _logger;
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

    public StationInformationHandler(
        ILogger<StationInformationHandler> logger)
    {
        _logger = logger;
    }


    public Task HandleAsync(
        string json,
        CancellationToken cancellationToken)
    {
        StationInformationDto? station =
            JsonSerializer.Deserialize<StationInformationDto>(
                json,
                JsonOptions);

        if (station is null)
        {
            throw new JsonException(
                "Station information message returned null.");
        }

        _logger.LogInformation(
            "Handled station information for station {StationId}.",
            station.StationId);

        return Task.CompletedTask;
    }
}
