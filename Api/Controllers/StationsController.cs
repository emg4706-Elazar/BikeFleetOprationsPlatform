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
}