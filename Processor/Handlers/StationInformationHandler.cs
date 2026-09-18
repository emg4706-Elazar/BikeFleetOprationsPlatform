using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Processor.Models;
using Processor.Data;
using Processor.Data.Entities;

namespace Processor.Handlers;

public class StationInformationHandler :
    IStationInformationHandler
{
    private readonly ILogger<StationInformationHandler> _logger;
    private readonly IDbContextFactory<BikeFleetDbContext> _contextFactory;
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

    public StationInformationHandler(
        IDbContextFactory<BikeFleetDbContext> contextFactory,
        ILogger<StationInformationHandler> logger)
    {
        _logger = logger;
        _contextFactory = contextFactory;
    }


    public async Task HandleAsync(
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

        StationEntity entity =
            MapToEntity(station);

        await using BikeFleetDbContext context =
            await _contextFactory.CreateDbContextAsync(
                cancellationToken);

        StationEntity? existing =
            await context.Stations.FindAsync(
                [entity.StationId],
                cancellationToken);

        if (existing is null)
        {
            context.Stations.Add(entity);
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
            "Saved station information for station {StationId}.",
            entity.StationId);
    }


    private static StationEntity MapToEntity(
        StationInformationDto dto)
    {
        return new StationEntity
        {
            StationId = dto.StationId,
            Name = dto.Name,
            ShortName = dto.ShortName,
            Longitude = (decimal)dto.Lon,
            Latitude = (decimal)dto.Lat,
            RegionId = dto.RegionId,
            Capacity = dto.Capacity,
            AndroidUri = dto.RentalUris?.Android,
            IosUri = dto.RentalUris?.Ios,
            WebUri = dto.RentalUris?.Web
        };
    }
}
