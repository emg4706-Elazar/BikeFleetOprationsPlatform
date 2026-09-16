using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Processor.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;


namespace Processor.Services;

public class KafkaConsumerService : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly KafkaOptions _kafkaOptions;

    public KafkaConsumerService(
        ILogger<KafkaConsumerService> logger,
        IOptions<KafkaOptions> options)
    {
        _logger = logger;

        _kafkaOptions = options.Value;

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

                _logger.LogInformation(
                    "Consumed message from topic {Topic}. " +
                    "Key: {Key}, Partition: {pPartition}, offset: {Offset}.",
                    result.Topic,
                    result.Message.Key,
                    result.Partition,
                    result.Offset);
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
                "kafka consumer error: {reason}",
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
