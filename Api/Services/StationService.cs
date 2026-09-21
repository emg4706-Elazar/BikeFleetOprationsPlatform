using Api.Models;
using Api.Repositories;
using Api.Data.Entities;
using Api.Mappings;

namespace Api.Services;

public class StationService : IStationService
{
    private readonly IStationInformationRepository _stationRepository;
    private readonly IStationStatusRepository _statusRepository;
    private readonly IStationHistoryRepository _historyRepository;


    public StationService(
        IStationInformationRepository stationRepository,
        IStationStatusRepository statusRepository,
        IStationHistoryRepository historyRepository)
    {
        _stationRepository = stationRepository;
        _statusRepository = statusRepository;
        _historyRepository = historyRepository;
    }


    // Endpoint 1 - Get all and allow filtering
    public async Task<List<StationFilterdResponseDto>> GetAllAsync(
            int? minAvailableBikes,
            bool? isRenting,
            bool? isReturning,
            CancellationToken cancellationToken)
    {
        List<StationEntity> stations =
            await _stationRepository.GetAllAsync(
                cancellationToken);

        List<StationFilterdResponseDto> response = [];

        foreach (StationEntity station in stations)
        {
            StationStatusDto? currentStatus =
                await _statusRepository.GetByStationIdAsync(
                    station.StationId,
                    cancellationToken);

            StationFilterdResponseDto dto =
                station.ToResponseDto(currentStatus);

            if (minAvailableBikes.HasValue &&
                (!dto.AvailableBikes.HasValue ||
                dto.AvailableBikes.Value < minAvailableBikes.Value))
            {
                continue;
            }

            if (isRenting.HasValue &&
                dto.IsRenting != isRenting.Value)
            {
                continue;
            }

            if (isReturning.HasValue &&
                dto.IsReturning != isReturning.Value)
            {
                continue;
            }

            response.Add(dto);
        }

        return response;
    }


    // Endpoint 2 - Station by id
    public async Task<StationDetailsResponseDto?>
        GetStationDetailsByIdAsync(
        string stationId,
        CancellationToken cancellationToken)
    {
        StationEntity? entity =
            await _stationRepository.GetByIdAsync(
                stationId,
                cancellationToken);

        if (entity is null)
        {
            return null;
        }

        StationStatusDto? currentStatus =
            await _statusRepository.GetByStationIdAsync(
                stationId,
                cancellationToken);

        return entity.ToDetailsResponseDto(currentStatus);
    }

    // Endpoint 3 - Currnet status
    public async Task<StationStatusResponseDto?>
        GetCurrentStatusAsync(
        string stationId,
        CancellationToken cancellationToken)
    {
        StationEntity? station =
            await _stationRepository.GetByIdAsync(
                stationId,
                cancellationToken);

        if (station is null)
        {
            return null;
        }

        StationStatusDto? status =
            await _statusRepository.GetByStationIdAsync(
                stationId,
                cancellationToken);

        if (status is null)
        {
            return null;
        }

        return status.ToResponseDto();
    }


    // Endpoint 4 - station history
    public async Task<List<StationHistoryResponseDto>?>
        GetHistoryAsync(
            string stationId,
            DateTime? from,
            DateTime? to,
            int? limit,
            CancellationToken cancellationToken)
    {
        StationEntity? station =
            await _stationRepository.GetByIdAsync(
                stationId,
                cancellationToken);

        if (station is null)
        {
            return null;
        }

        List<StationStatusHistory> history =
            await _historyRepository.GetAsync(
                stationId,
                from,
                to,
                limit,
                cancellationToken);

        return history
            .Select(record =>
            record.ToResponseDto())
            .ToList();
    }
}