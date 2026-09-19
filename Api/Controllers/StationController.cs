using Microsoft.AspNetCore.Mvc;
using Api.Repositories;
using Api.Models;
using Api.Data.Entities;
using Api.Mappings;

namespace Api.Controllers;

[ApiController]
[Route("/api/stations")]
public class StationController : ControllerBase
{
    private readonly IStationInformationRepository _repository;


    public StationController(
        IStationInformationRepository repository)
    {
        _repository = repository;
    }


    [HttpGet]
    public async Task<ActionResult<
        List<StationInfoResponseDto>>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        List<StationEntity> stations =
            await _repository.GetAllAsync(
                cancellationToken);

        List<StationInfoResponseDto> response = stations
            .Select(station =>
            station.ToResponseDto())
            .ToList();

        return Ok(response);
    }


    [HttpGet("{stationId}")]
    public async Task<ActionResult<
        StationInfoResponseDto>> GetByIdAsync(
        string stationId,
        CancellationToken cancellationToken)
    {
        StationEntity? station =
            await _repository.GetByIdAsync(
                stationId,
                cancellationToken);

        if (station is null)
        {
            return NotFound();
        }

        return Ok(station.ToResponseDto());
    }
}