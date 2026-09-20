using Api.Models;

namespace Api.Repositories;

public interface IStationStatusRepository
{
    Task<StationStatusDto?> GetByStationIdAsync(
        string stationId,
        CancellationToken cancellationToken);
}
