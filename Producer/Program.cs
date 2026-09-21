using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Producer.Services;
using Producer.Configuration;

namespace Producer;

public class Program
{
    public static async Task Main(string[] args)
    {
        HostApplicationBuilder builder =
            Host.CreateApplicationBuilder(args);

        builder.Services.AddHttpClient();

        builder.Services
            .AddOptions<KafkaOptions>()
            .Bind(builder.Configuration.GetSection("kafka"))
            .Validate(
                options => !string.IsNullOrWhiteSpace(
                    options.BootstrapServers),
                "Kafka:BootstrapServers is required.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(
                    options.ClientId),
                "Kafka:ClientId is required.")
            .ValidateOnStart();

        builder.Services.AddSingleton<
            IKafkaProducerService,
            KafkaProducerService>();

        builder.Services.AddHostedService<StationStatusService>();
        builder.Services.AddHostedService<StationInformationService>();
        builder.Services.AddHostedService<VehicleTypeService>();

        using IHost host = builder.Build();

        await host.RunAsync();
    }
}
