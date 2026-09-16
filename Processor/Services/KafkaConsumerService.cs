using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Processor.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Processor.Handlers;


namespace Processor.Services;

public class KafkaConsumerService : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly KafkaOptions _kafkaOptions;
    private readonly IStationInformationHandler _stationInformationHandler;
    private readonly IVehicleTypesHandler _vehicleTypesHandler;
    private readonly IStationStatusHandler _stationStatusHandler;

    public KafkaConsumerService(
        ILogger<KafkaConsumerService> logger,
        IOptions<KafkaOptions> options,
        IStationInformationHandler stationInformationHandler,
        IVehicleTypesHandler vehicleTypesHandler,
        IStationStatusHandler stationStatusHandler)
    {
        _logger = logger;
        _kafkaOptions = options.Value;
        _stationInformationHandler = stationInformationHandler;
        _vehicleTypesHandler = vehicleTypesHandler;
        _stationStatusHandler = stationStatusHandler;

        ConsumerConfig config = new()
        {
            BootstrapServers = _kafkaOptions.BootstrapServers,
            GroupId = _kafkaOptions.GroupId,
            ClientId = _kafkaOptions.ClientId,
            EnableAutoCommit = false,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<
            string, string>(config)
            .Build();
    }


    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        await Task.Yield();

        string[] topics =
        [
            _kafkaOptions.Topics.StationInformation,
            _kafkaOptions.Topics.StationStatus,
            _kafkaOptions.Topics.VehicleTypes
        ];

        _consumer.Subscribe(topics);

        _logger.LogInformation(
            "Kafka consumer subscribed to {TopicCount} topics",
            topics.Length);

        try
        {
            
            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<string, string> result =
                    _consumer.Consume(stoppingToken);

                if (result.Message.Value is null)
                {
                    _logger.LogWarning(
                        "Received null message from topic {Topic}, key {Key}.",
                        result.Topic,
                        result.Message.Key);
                    continue;
                }


                switch (result.Topic)
                {
                    case var topic
                        when topic == 
                        _kafkaOptions.Topics.StationInformation:

                        await _stationInformationHandler
                            .HandleAsync(
                            result.Message.Value,
                            stoppingToken);
                        break;

                    case var topic
                        when topic ==
                        _kafkaOptions.Topics.StationStatus:

                        await _stationStatusHandler
                            .HandleAsync(
                            result.Message.Value,
                            stoppingToken);

                        break;

                    case var topic
                        when topic ==
                        _kafkaOptions.Topics.VehicleTypes:

                        await _vehicleTypesHandler
                            .HandleAsync(
                            result.Message.Value,
                            stoppingToken);

                        break;

                    default:
                        _logger.LogWarning(
                            "Received message from unknown topic {Topic}.",
                            result.Topic);

                        break;
                }
            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "Kafka consumer is stopping");
        }
        catch (ConsumeException ex)
        {
            _logger.LogError(
                ex,
                "kafka consumer error: {Reason}",
                ex.Error.Reason);
        }
        finally
        {
            _consumer.Close();
        }
    }


    public override void Dispose()
    {
        _consumer.Dispose();
        base.Dispose();
    }
}
