


using Api.Data;
using Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories;

public class StationInformationRepository : IStationInformationRepository
{
    private readonly BikeFleetDbContext _context;

    public StationInformationRepository(
        BikeFleetDbContext context)
    {
        _context = context;
    }

    // Get all station information
    public async Task<List<StationEntity>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Stations
            .AsNoTracking()
            .OrderBy(station => station.Name)
            .ToListAsync(cancellationToken);
    }

    // Get station information by id
    public async Task<StationEntity?> GetByIdAsync(
        string stationId,
        CancellationToken cancellationToken)
    {
        return await _context.Stations
            .AsNoTracking()
            .FirstOrDefaultAsync(
                station => station.StationId == stationId,
                cancellationToken);
    }

}
