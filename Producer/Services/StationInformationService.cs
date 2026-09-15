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

            _logger.LogInformation(
                "Received {StationCount} station information records",
                stationInformationResponse.Data.Stations.Count
            );

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



}
