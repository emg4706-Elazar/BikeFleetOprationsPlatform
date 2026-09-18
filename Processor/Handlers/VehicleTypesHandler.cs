using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Processor.Data;
using Processor.Data.Entities;
using Processor.Models;
using System.Text.Json;

namespace Processor.Handlers;

public class VehicleTypesHandler : IVehicleTypesHandler
{
    private readonly ILogger<VehicleTypesHandler> _logger;
    private readonly IDbContextFactory<BikeFleetDbContext>
        _contextFactory;
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

    public VehicleTypesHandler(
        IDbContextFactory<BikeFleetDbContext>
        contextFactory,
    ILogger<VehicleTypesHandler> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }


    public async Task HandleAsync(
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

        VehicleTypeEntity entity =
            MapToEntity(vehicle);

        await using var context =
            await _contextFactory
            .CreateDbContextAsync(
                cancellationToken);

        VehicleTypeEntity? existing =
            await context.VehicleTypes
            .FindAsync([entity.VehicleTypeId],
            cancellationToken);

        if (existing is null)
        {
            context.VehicleTypes.Add(entity);
        }
        else
        {
            context.Entry(existing)
                .CurrentValues
                .SetValues(entity);
        }

        await context.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "Saved vehicle type for vehicle {VehicleId}.",
            entity.VehicleTypeId);
    }


    private static VehicleTypeEntity MapToEntity(
        VehicleTypeDto dto)
    {
        return new VehicleTypeEntity
        {
            VehicleTypeId = dto.VehicleTypeId,
            FormFactor = dto.FormFactor,
            PropulsionType = dto.PropulsionType,
            MaxRangeMeters = (decimal?)dto.MaxRangeMeters
        };
    }
}
