using Microsoft.AspNetCore.Mvc;
using Api.Services;
using Api.Models;
using Api.Data.Entities;
using Api.Mappings;

namespace Api.Controllers;

[ApiController]
[Route("api/stations")]
public class StationsController : ControllerBase
{
    private readonly IStationService _stationService;


    public StationsController(
        IStationService stationService)
    {
        _stationService = stationService;
    }


    [HttpGet]
    public async Task<ActionResult<
        List<StationFilterdResponseDto>>> GetAllAsync(
        int? minAvailableBikes,
        bool? isRenting,
        bool? isReturning,
        CancellationToken cancellationToken)
    {
        if (minAvailableBikes < 0)
        {
            return BadRequest(
                "minAvailableBikes nust be zero or greater.");
        }

        List<StationFilterdResponseDto> response =
            await _stationService.GetAllAsync(
                minAvailableBikes,
                isRenting,
                isReturning,
                cancellationToken);


        return Ok(response);
    }


    [HttpGet("{stationId}")]
    public async Task<ActionResult<
        StationDetailsResponseDto>> GetByIdAsync(
        string stationId,
        CancellationToken cancellationToken)
    {
        StationDetailsResponseDto? response =
            await _stationService.GetStationDetailsByIdAsync(
                stationId,
                cancellationToken);

        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }


    [HttpGet("{id}/status")]
    public async Task<ActionResult<StationStatusResponseDto>>
        GetCurrentStatus(string id,
        CancellationToken cancellationToken)
    {
        StationStatusResponseDto? status =
            await _stationService.GetCurrentStatusAsync(
                id,
                cancellationToken);

        if (status is null)
        {
            return NotFound();
        }

        return Ok(status);
    }


    // Endpoint 4 - station history
    [HttpGet("{id}/history")]
    public async Task<ActionResult<
        List<StationHistoryResponseDto>>> GetHistoryAsync(
        string id,
        DateTime? from,
        DateTime? to,
        int? limit,
        CancellationToken cancellationToken)
    {
        if (limit.HasValue && limit.Value <= 0)
        {
            return BadRequest(
                "Limit must be greater than zero.");
        }

        if (from.HasValue &&
            to.HasValue &&
            from.Value > to.Value)
        {
            return BadRequest(
                "from cannot be later than to");
        }

        List<StationHistoryResponseDto>? response =
            await _stationService.GetHistoryAsync(
                id,
                from,
                to,
                limit,
                cancellationToken);

        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }
}