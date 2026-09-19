using Api.Configuration;
using Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StackExchange.Redis;
using Api.Repositories;
using Api.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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
builder.Services.AddDbContextPool<
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


// Register the station information repository
builder.Services.AddScoped<IStationInformationRepository,
    StationInformationRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
