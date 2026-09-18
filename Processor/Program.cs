using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Processor.Configuration;
using Processor.Services;
using Processor.Handlers;
using Processor.Models;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using Processor.Data;

namespace Processor;

public class Program
{
    public static async Task Main(string[] args)
    {
        HostApplicationBuilder builder =
            Host.CreateApplicationBuilder(args);

        // Add kakfa configuration to DI
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

        // Register the Mongo options in the DI
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

        // Register the Consumer service
        builder.Services.AddHostedService<
            KafkaConsumerService>();

        // Register the station information handler
        builder.Services.AddSingleton<
            IStationInformationHandler,
            StationInformationHandler>();

        // Register the vehicle type handler
        builder.Services.AddSingleton<
            IVehicleTypesHandler,
            VehicleTypesHandler>();

        // Register the station status handler
        builder.Services.AddSingleton<
            IStationStatusHandler,
            StationStatusHandler>();

        // Add mongo configuration into DI
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

        // Register the mongo connection represents the collection
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
                client.GetDatabase(options.DatabaseName);

                return database
                .GetCollection<StationStatusHistory>(
                    options.CollectionName);
            });

        // Add the mysql configuration into the DI
        builder.Services.AddOptions<MySqlOptions>()
            .Bind(builder.Configuration.GetSection("MySql"))
            .Validate(options =>
                !string.IsNullOrWhiteSpace(options.ConnectionString),
                "MySql:ConnectionString is required")
            .ValidateOnStart();

        // Register the mysql connection represents the database
        builder.Services.AddPooledDbContextFactory<
            BikeFleetDbContext>(
            (serviceProvider, optionsBuilder) =>
            {
                MySqlOptions mySqlOptions =
                    serviceProvider
                        .GetRequiredService<
                            IOptions<MySqlOptions>>()
                            .Value;

                optionsBuilder.UseMySql(
                    mySqlOptions.ConnectionString,
                    new MySqlServerVersion(
                        new Version(8, 4, 0)));
            });

        // Register the redis options into the DI
        builder.Services.AddOptions<RedisOptions>()
            .Bind(builder.Configuration.GetSection("Redis"))
            .Validate(options =>
                !string.IsNullOrWhiteSpace(options.ConnectionString),
                "Redis:ConnectionString is required")
            .ValidateOnStart();

        // Register the Redis connection into the DI
        builder.Services.AddSingleton<IConnectionMultiplexer>(
            serviceProvider =>
            {
                RedisOptions options =
                    serviceProvider.GetRequiredService<
                        IOptions<RedisOptions>>()
                        .Value;

                return ConnectionMultiplexer.Connect(
                    options.ConnectionString);
            });

        // Add Redis database into the DI
        builder.Services.AddSingleton<IDatabase>(
            serviceProvider =>
            {
                IConnectionMultiplexer connection =
                    serviceProvider.GetRequiredService<
                        IConnectionMultiplexer>();


                return connection.GetDatabase();
            });

        using IHost host = builder.Build();


        // Run the migrations in mysql
        await using (AsyncServiceScope scope =
            host.Services.CreateAsyncScope())
        {
            IDbContextFactory<BikeFleetDbContext> contextFactory =
                scope.ServiceProvider.GetRequiredService<
                    IDbContextFactory<BikeFleetDbContext>>();

            await using BikeFleetDbContext context =
                await contextFactory.CreateDbContextAsync();

            await context.Database.MigrateAsync();
        }


        await host.RunAsync();
    }
}
