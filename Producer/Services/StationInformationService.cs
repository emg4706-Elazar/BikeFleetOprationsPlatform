using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Producer.Models;

namespace Producer.Services;

public class StationInformationService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
    private const string ApiAddress =
        "https://gbfs.lyft.com/gbfs/2.3/bkn/en/station_information.json";
    private readonly ILogger<StationInformationService> _logger;

    public StationInformationService(
        IHttpClientFactory httpClientFactory,
        ILogger<StationInformationService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<StationInformationResponseDto?>
        FetchAsync(CancellationToken cancellationToken)
    {       
        try
        {
            HttpClient client = _httpClientFactory
            .CreateClient();

            var stationInformationResponse =
            await client.GetFromJsonAsync<
                StationInformationResponseDto>(
                    ApiAddress,
                    JsonOptions,
                    cancellationToken);

            if (stationInformationResponse is null)
            {
                _logger.LogWarning(
                    "StationInformation URL returned null.");

                return null;
            }

            var validStations =
                stationInformationResponse.Data.Stations
                .Where(IsValid)
                .ToList();

            int invalidCount =
                stationInformationResponse.Data.Stations.Count -
                validStations.Count;

            stationInformationResponse.Data.Stations = validStations;

            _logger.LogInformation(
                "Received {StationCount} valid station information records",
                validStations.Count
            );

            _logger.LogInformation(
                "{InvalidCount} Invalid station information records received.",
                invalidCount);

            return stationInformationResponse;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex,
                "Failed to fetch station information.");
        
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(
                ex,
                "Failed to deserialize station information.");

            return null;
        }
    }


    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Station information background service started.");
        using PeriodicTimer timer =
            new(TimeSpan.FromHours(1));

        try
        {
            await FetchAsync(stoppingToken);
            
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await FetchAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "station information background service stopped.");
        }
    }


    private static bool IsValid(
        StationInformationDto stationInformation)
    {
        if (string.IsNullOrWhiteSpace(stationInformation.StationId))
            return false;

        if (stationInformation.Lat < -90 ||
            stationInformation.Lat > 90)
            return false;

        if (stationInformation.Lon < -180 ||
            stationInformation.Lon > 180)
            return false;

        if (stationInformation.Capacity < 0)
            return false;

        if (string.IsNullOrWhiteSpace(stationInformation.Name))
            return false;

        return true;
    }
}
