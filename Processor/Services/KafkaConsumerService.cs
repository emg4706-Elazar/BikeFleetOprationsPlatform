using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Processor.Configuration;
using Microsoft.Extensions.Options;


namespace Processor.Services;

public class KafkaConsumerService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<KafkaConsumerService> _logger;

    public KafkaConsumerService(
        ILogger<KafkaConsumerService> logger,
        IOptions<KafkaOptions> options)
    {
        _logger = logger;

        KafkaOptions kafkaOptions = options.Value;

        ConsumerConfig config = new()
        {
            BootstrapServers = kafkaOptions.BootstrapServers,
            GroupId = kafkaOptions.GroupId,
            EnableAutoCommit = false,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<
            string, string>(config)
            .Build();
    }


    public async Task<IEnumerable<T>> ConsumeAsync<T>(
        string topicName,
        CancellationToken cancellationToken)
    {

    }
}
