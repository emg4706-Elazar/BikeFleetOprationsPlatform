using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Processor.Configuration;
using Processor.Services;
using Processor.Handlers;

namespace Processor;

public class Program
{
    public static async Task Main(string[] args)
    {
        HostApplicationBuilder builder =
            Host.CreateApplicationBuilder(args);

        builder.Services.AddOptions<KafkaOptions>()
            .Bind(builder.Configuration.GetSection("Kafka"))
            .Validate(options =>
                !string.IsNullOrWhiteSpace(options.BootstrapServers),
                    "Kafka:BootstrapServers is required.")
            .Validate(options =>
                !string.IsNullOrWhiteSpace(options.GroupId),
                    "Kafka:GroupId is required.")
            .Validate(options =>
                !string.IsNullOrWhiteSpace(options.ClientId),
                    "Kafka:ClientId is required")
            .Validate(options =>
                !string.IsNullOrWhiteSpace(
                    options.Topics.StationInformation) &&
                !string.IsNullOrWhiteSpace(
                    options.Topics.StationStatus) &&
                !string.IsNullOrWhiteSpace(
                    options.Topics.VehicleTypes),
                    "All Kafka topic names are required.")
            .ValidateOnStart();

        builder.Services.AddHostedService<KafkaConsumerService>();

        builder.Services.AddSingleton<
            IStationInformationHandler,
            StationInformationHandler>();

        builder.Services.AddSingleton<
            IVehicleTypesHandler,
            VehicleTypesHandler>();

        builder.Services.AddSingleton<
            IStationStatusHandler,
            StationStatusHandler>();

        using IHost host = builder.Build();

        await host.RunAsync();
    }
}
