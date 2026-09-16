using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Processor.Configuration;

namespace Producer;

public class Program
{
    public static void Main(string[] args)
    {
        HostApplicationBuilder builder =
            Host.CreateApplicationBuilder(args);

        builder.Services.AddOptions<KafkaOptions>()
            .Bind(builder.Configuration.GetSection("Kafka"))
            .Validate(options =>
                string.IsNullOrWhiteSpace(options.BootstrapServers),
                    "Kafka:BootstrapServers is required.")
            .Validate(options =>
                string.IsNullOrWhiteSpace(options.GroupId),
                    "Kafka:GroupId is required.")
            .ValidateOnStart();


    }
}
