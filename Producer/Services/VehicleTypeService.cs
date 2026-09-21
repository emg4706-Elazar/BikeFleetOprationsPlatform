using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Producer.Models;

namespace Producer.Services;

public class VehicleTypeService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IKafkaProducerService _kafkaProducer;
    private const string ApiAddress =
        "https://gbfs.lyft.com/gbfs/2.3/bkn/en/vehicle_types.json";
    private const string TopicName = "bike.vehicle-types";
    private readonly ILogger<VehicleTypeService> _logger;
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

    public VehicleTypeService(
        IHttpClientFactory httpClientFactory,
        IKafkaProducerService kafkaProducer,
        ILogger<VehicleTypeService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _kafkaProducer = kafkaProducer;
        _logger = logger;
    }


    public async Task<VehicleTypeResponseDto?> FetchAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            HttpClient client = _httpClientFactory
                .CreateClient();

            var vehicleTypeResponse =
                await client.GetFromJsonAsync<VehicleTypeResponseDto>(
                    ApiAddress,
                    JsonOptions,
                    cancellationToken);

            if (vehicleTypeResponse is null)
            {
                _logger.LogWarning(
                    "VehicleType URL returned null.");
                return null;
            }

            var validVehicles =
                vehicleTypeResponse.Data.VehicleTypes
                .Where(IsValid)
                .ToList();


            int invalidCount =
                vehicleTypeResponse.Data.VehicleTypes.Count -
                validVehicles.Count;

            vehicleTypeResponse.Data.VehicleTypes = validVehicles;

            _logger.LogInformation(
                "Received {ValidCount} vehicle types records.",
                validVehicles.Count);

            _logger.LogInformation(
                "{InvalidCount} invalid vehicle types records.",
                invalidCount);


            await _kafkaProducer.PublishBatchAsync(
                TopicName,
                validVehicles,
                vehicle => vehicle.VehicleTypeId,
                cancellationToken);

            return vehicleTypeResponse;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex,
                "Failed to fetch vehicle types ");

            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(
                ex,
                "Failed to deserialize vehicle types");

            return null;
        }
    }


    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Vehicle types background service started");
        try
        {
            using PeriodicTimer timer =
                new(TimeSpan.FromHours(1));

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
                "Vehicle types background service stopped");
        }
    }


    // Validates required fields according to the GBFS vehicle_types schema.
    private static bool IsValid(VehicleTypeDto vehicle)
    {
        if (string.IsNullOrWhiteSpace(vehicle.VehicleTypeId))
            return false;

        if (string.IsNullOrWhiteSpace(vehicle.FormFactor))
            return false;

        if (string.IsNullOrWhiteSpace(vehicle.PropulsionType))
            return false;

        if (vehicle.MaxRangeMeters < 0)
            return false;

        bool isHumanPowered =
            string.Equals(
                vehicle.PropulsionType,
                "human",
                StringComparison.OrdinalIgnoreCase);

        if (!isHumanPowered &&
            vehicle.MaxRangeMeters is null)
        {
            return false;
        }

        return true;


    }
}
