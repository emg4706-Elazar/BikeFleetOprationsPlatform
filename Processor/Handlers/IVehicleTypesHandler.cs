
namespace Processor.Handlers;

public interface IVehicleTypesHandler
{
    Task HandleAsync(
        string json,
        CancellationToken cancellationToken);
}