using Api.Models;
using MongoDB.Driver;

namespace Api.Repositories;

public class StationHistoryRepository : 
    IStationHistoryRepository
{
    private readonly IMongoCollection<StationStatusHistory>
        _collection;

    public StationHistoryRepository(
        IMongoCollection<StationStatusHistory>
        collection)
    {
        _collection = collection;
    }
    public async Task<List<StationStatusHistory>> GetAsync(
        string stationId,
        DateTime? from,
        DateTime? to,
        int? limit,
        CancellationToken cancellationToken)
    {
        FilterDefinitionBuilder<StationStatusHistory>
            filterBuilder =
            Builders<StationStatusHistory>.Filter;

        FilterDefinition<StationStatusHistory> filter =
            filterBuilder.Eq(
                history => history.StationId,
                stationId);

        if (from.HasValue)
        {
            filter &= filterBuilder.Gte(
                history => history.RecordedAt,
                from.Value);
        }

        if (to.HasValue)
        {
            filter &= filterBuilder.Lte(
                history => history.RecordedAt,
                to.Value);
        }

        IFindFluent<StationStatusHistory,
            StationStatusHistory> query =
            _collection
                .Find(filter)
                .SortByDescending(
                history => history.RecordedAt);

        if (limit.HasValue)
        {
            query = query.Limit(limit.Value);
        }

        return await query.ToListAsync(
            cancellationToken);
    }
}
