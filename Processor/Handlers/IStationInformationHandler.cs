
namespace Processor.Handlers;

public interface IStationInformationHandler
{
    Task HandleAsync(
        string json,
        CancellationToken cancellationToken);
}
