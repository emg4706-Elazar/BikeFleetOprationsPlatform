using Microsoft.Extensions.Logging;
using System.Text.Json;
using Processor.Models;

namespace Processor.Handlers;

public class VehicleTypesHandler : IVehicleTypesHandler
{
    private readonly ILogger<VehicleTypesHandler> _logger;
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

    public VehicleTypesHandler(
        ILogger<VehicleTypesHandler> logger)
    {
        _logger = logger;
    }


    public Task HandleAsync(
        string json,
        CancellationToken cancellationToken)
    {
        VehicleTypeDto? vehicle =
            JsonSerializer.Deserialize<
                VehicleTypeDto>(
                json,
                JsonOptions);

        if (vehicle is null)
        {
            throw new JsonException(
                "Vehicle Type message returned null");
        }

        _logger.LogInformation(
            "Handled vehicle type message for vehicle {VehicleId}",
            vehicle.VehicleTypeId);

        return Task.CompletedTask;
    }
}
