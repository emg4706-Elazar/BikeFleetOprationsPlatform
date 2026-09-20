using Api.Models;

namespace Api.Services
{
    public interface IStationService
    {
        Task<List<StationFilterdResponseDto>> GetAllAsync(
            int? minAvailableBikes,
            bool? isRenting,
            bool? isReturning,
            CancellationToken cancellationToken);

        Task<StationDetailsResponseDto?>
        GetStationDetailsByIdAsync(
        string stationId,
        CancellationToken cancellationToken);

        Task<StationStatusResponseDto?>
        GetCurrentStatusAsync(
        string stationId,
        CancellationToken cancellationToken);
    }
}
