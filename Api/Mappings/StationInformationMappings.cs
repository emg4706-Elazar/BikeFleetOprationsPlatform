using Api.Data.Entities;
using Api.Models;

namespace Api.Mappings;

public static class StationInformationMappings
{
    public static StationInformationResponseDto
        ToInformationResponseDto( this StationEntity entity)
    {
        return new StationInformationResponseDto
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
