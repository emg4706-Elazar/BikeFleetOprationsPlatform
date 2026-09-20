using Api.Data.Entities;
using Api.Models;

namespace Api.Mappings;

public static class StationMappings
{
    public static StationFilterdResponseDto ToResponseDto(
        this StationEntity status,
        StationStatusDto? currentStatus)
    {
        return new StationFilterdResponseDto
        {
            Id = status.StationId,
            Name = status.Name,
            Longitude = status.Longitude,
            Latitude = status.Latitude,
            Capacity = status.Capacity,

            AvailableBikes = currentStatus?.NumBikesAvailable,
            AvailableDocks = currentStatus?.NumDocksAvailable,
            
            IsRenting = currentStatus is null
                ? null
                : currentStatus.IsRenting == 1,

            IsReturning = currentStatus is null
                ? null
                : currentStatus.IsReturning == 1
        };
    }
}
