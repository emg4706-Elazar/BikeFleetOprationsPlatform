using Api.Data.Entities;
using Api.Models;

namespace Api.Mappings;

public static class StationMappings
{
    public static StationInfoResponseDto ToResponseDto(
        this StationEntity entity)
    {
        return new StationInfoResponseDto
        {
            StationId = entity.StationId,
            Name = entity.Name,
            ShortName = entity.ShortName,
            Longitude = entity.Longitude,
            Latitude = entity.Latitude,
            RegionId = entity.RegionId,
            Capacity = entity.Capacity,
            AndroidUri = entity.AndroidUri,
            IosUri = entity.IosUri,
            WebUri = entity.WebUri
        };
    }
}
