using Api.Models;


namespace Api.Repositories;

public interface IStationHistoryRepository
{
    Task<List<StationStatusHistory>> GetAsync(
        string stationId,
        DateTime? from,
        DateTime? to,
        int? limit,
        CancellationToken cancellationToken);
}
