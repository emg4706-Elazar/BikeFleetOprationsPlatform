


namespace Api.Models;

public class StationFilterdResponseDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal Longitude { get; set; }
    public decimal Latitude { get; set; }
    public int Capacity { get; set; }


    public int? AvailableBikes { get; set; }
    public int? AvailableDocks { get; set; }
    public bool? IsRenting { get; set; }
    public bool? IsReturning { get; set; }
}
