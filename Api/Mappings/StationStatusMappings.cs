

using Api.Models;

namespace Api.Mappings;

public static class StationStatusMappings
{
    public static StationStatusResponseDto ToResponseDto(
        this StationStatusDto status)
    {
        return new StationStatusResponseDto
        {
            StationId = status.StationId,
            AvailableBikes = status.NumBikesAvailable,
            AvailableDocks = status.NumDocksAvailable,
            IsRenting = status.IsRenting == 1,
            IsReturing = status.IsReturning == 1,
            LastReported = status.LastReported
        };
    }
}
