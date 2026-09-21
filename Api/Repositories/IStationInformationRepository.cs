using Api.Data.Entities;

namespace Api.Repositories;

public interface IStationInformationRepository
{
    Task<List<StationEntity>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<StationEntity?> GetByIdAsync(string stationId,
        CancellationToken cancellationToken);
}
