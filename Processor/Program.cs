using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Processor.Configuration;
using Processor.Services;
using Processor.Handlers;
using Processor.Models;

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


        builder.Services.AddOptions<MongoOptions>()
            .Bind(builder.Configuration.GetSection("Mongo"))
            .Validate(options =>
                !string.IsNullOrWhiteSpace(options.ConnectionString),
                "Mongo:ConnectionString is required")
            .Validate(options =>
                !string.IsNullOrWhiteSpace(options.DatabaseName),
                "Mongo:DatabaseName is required")
            .Validate(options =>
                !string.IsNullOrWhiteSpace(options.CollectionName),
                "Mongo:CollectionName is required")
            .ValidateOnStart();


        builder.Services.AddHostedService<
            KafkaConsumerService>();

        builder.Services.AddSingleton<
            IStationInformationHandler,
            StationInformationHandler>();

        builder.Services.AddSingleton<
            IVehicleTypesHandler,
            VehicleTypesHandler>();

        builder.Services.AddSingleton<
            IStationStatusHandler,
            StationStatusHandler>();

        builder.Services.AddSingleton<IMongoClient>(
            serviceProvider =>
            {
                MongoOptions options =
                    serviceProvider.GetRequiredService<
                        IOptions<MongoOptions>>()
                        .Value;

                return new MongoClient(
                    options.ConnectionString);
            });

        builder.Services.AddSingleton<IMongoCollection<StationStatusHistory>>(
            serviceProvider =>
            {
                IMongoClient client =
                serviceProvider.GetRequiredService<IMongoClient>();

                MongoOptions options =
                serviceProvider
                    .GetRequiredService<IOptions<MongoOptions>>()
                    .Value;

                IMongoDatabase database =
                client.GetDatabase(options.CollectionName);

                return database
                .GetCollection<StationStatusHistory>(
                    options.CollectionName);
            });

        using IHost host = builder.Build();


        await host.RunAsync();
    }
}
