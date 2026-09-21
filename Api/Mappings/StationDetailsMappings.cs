using Api.Models;
using Api.Data.Entities;



namespace Api.Mappings;

public static class StationDetailsMappings
{
    public static StationDetailsResponseDto ToDetailsResponseDto(
        this StationEntity  entity,
        StationStatusDto? currentStatus)
    {
        return new StationDetailsResponseDto
        {
            Information = entity.ToInformationResponseDto(),
            CurrentStatus = currentStatus?.ToResponseDto()
        };
    }
}
