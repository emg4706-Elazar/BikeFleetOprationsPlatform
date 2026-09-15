using Microsoft.Extensions.Logging;
using Producer.Models;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Hosting;

namespace Producer.Services;

public class StationStatusService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private const string ApiAddress =
    "https://gbfs.lyft.com/gbfs/2.3/bkn/en/station_status.json";
    private readonly ILogger<StationStatusService> _logger;

    public StationStatusService(
        IHttpClientFactory httpClientFactory,
        ILogger<StationStatusService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<StationStatusResponseDto?>
        FetchAsync(CancellationToken cancellationToken)
    {
        try
        {
            HttpClient client = 
                _httpClientFactory.CreateClient();

            StationStatusResponseDto? stationStatusResponse =
                await client.GetFromJsonAsync<StationStatusResponseDto>(
                    ApiAddress,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web),
                    cancellationToken);

            if (stationStatusResponse is null)
            {
                _logger.LogWarning(
                    "StationStatus URL returned null.");
                return null;
            }

            var allStations = stationStatusResponse.Data.Stations;

            var validStations =
                allStations
                    .Where(IsValid)
                    .ToList();

            int invalidCount =
                allStations.Count - validStations.Count;

            stationStatusResponse.Data.Stations = validStations;

            _logger.LogInformation(
                "{InvalidCount} Invalid station statuses.",
                invalidCount);

            _logger.LogInformation(
                "{ValidCount} valid station statuses.",
                validStations.Count);

            return stationStatusResponse;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex,
                "Failed to fetch station statuses");

                return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(
                ex,
                "Failed to deserialize station statuses"
            );

            return null;
        }
    }


    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Station status background service started");

        using PeriodicTimer timer =
            new(TimeSpan.FromSeconds(60));

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
                "Station status background service stopped");
        }
    }


    // Valid Data
    private static bool IsValid(StationStatusDto station)
    {
        if (string.IsNullOrWhiteSpace(station.StationId))
            return false;

        if (station.IsInstalled is not (0 or 1))
            return false;

        if (station.IsRenting is not (0 or 1))
            return false;

        if (station.IsReturning is not (0 or 1))
            return false;

        if (station.VehicleTypesAvailable is null)
            return false;

        if (station.VehicleTypesAvailable.Any(
            vehicleType =>
                string.IsNullOrWhiteSpace(vehicleType.VehicleTypeId) ||
                vehicleType.Count < 0))
        {
            return false;
        }


        if (station.NumScootersAvailable is not null)
        {
            if (station.NumScootersAvailable < 0)
                return false;
        }

        if (station.NumScootersUnavailable is not null)
        {
            if (station.NumScootersUnavailable < 0)
                return false;
        }

        if (station.NumBikesAvailable < 0)
            return false;

        if (station.NumBikesDisabled < 0)
            return false;

        if (station.NumDocksAvailable < 0)
            return false;

        if (station.NumDocksDisabled < 0)
            return false;

        if (station.NumEbikesAvailable < 0)
            return false;

        if (station.LastReported < 0)
            return false;

        return true;
    }
}