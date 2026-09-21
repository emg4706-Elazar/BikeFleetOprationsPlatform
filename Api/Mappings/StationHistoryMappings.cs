using Api.Models;


namespace Api.Mappings;

public static class StationHistoryMappings
{
    public static StationHistoryResponseDto ToResponseDto(
        this StationStatusHistory history)
    {
        return new StationHistoryResponseDto
        {
            Timestamp = history.RecordedAt,
            AvailableBikes =
                history.NumBikesAvailable,
            AvailableDocks =
                history.NumDocksAvailable
        };
    }
}
