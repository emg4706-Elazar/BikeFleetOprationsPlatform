

namespace Api.Models;

public class StationStatusResponseDto
{
    public string StationId { get; set; } = null!;
    public int AvailableBikes { get; set; }
    public int AvailableDocks { get; set; }
    public bool IsRenting { get; set; }
    public bool IsReturning { get; set; }
    public long LastReported { get; set; }
}
