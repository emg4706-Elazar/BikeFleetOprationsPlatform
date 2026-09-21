


namespace Api.Models;

public class StationHistoryResponseDto
{
    public DateTime Timestamp { get; set; }
    public int AvailableBikes { get; set; }
    public int AvailableDocks { get; set; }
}
