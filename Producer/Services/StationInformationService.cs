using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Producer.Models;

namespace Producer.Services;

public class StationInformationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private const string ApiAddress =
        "https://gbfs.lyft.com/gbfs/2.3/bkn/en/station_information.json";
    private readonly ILogger<StationInformationService> _logger;

    public StationInformationService(
        IHttpClientFactory httpClientFactory,
        ILogger<StationInformationService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<StationStatusResponseDto?>
        FetchAsync(CancellationToken cancellationToken)
    {
        HttpClient client = _httpClientFactory
            .CreateClient();

        var stationInformationResponse =
            await client.GetFromJsonAsync<
                StationStatusResponseDto>(
                    ApiAddress,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web),
                    cancellationToken);

        if (stationInformationResponse is null)
        {
            _logger.LogWarning(
                "StationInformation URL returned null.");

            return null;
        }

        return stationInformationResponse;
    }

}
