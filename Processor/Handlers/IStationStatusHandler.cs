
namespace Processor.Handlers;

public interface IStationStatusHandler
{
    Task HandleAsync(
        string json,
        CancellationToken cancellationToken);
}
