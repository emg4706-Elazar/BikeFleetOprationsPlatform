

namespace Api.Models;

public class StationDetailsResponseDto
{
    public StationInformationResponseDto Information { get; set; } = null!;
    public StationStatusResponseDto? CurrentStatus { get; set; }
}
