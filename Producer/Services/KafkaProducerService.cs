using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using Producer.Configuration;
using Microsoft.Extensions.Options;

namespace Producer.Services;

public class KafkaProducerService :
    IKafkaProducerService,
    IDisposable
{
    // Setup Configuration   
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducerService> _logger;
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = 
            JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = 
            JsonIgnoreCondition.WhenWritingNull
        };

    public KafkaProducerService(
        ILogger<KafkaProducerService> logger,
        IOptions<KafkaOptions> options)
    {
        _logger = logger;

        KafkaOptions kafkaOptions = options.Value;

        ProducerConfig config = new()
        {
            BootstrapServers = kafkaOptions.BootstrapServers,
            ClientId = kafkaOptions.ClientId,
            EnableIdempotence = true
        };

        _producer = new ProducerBuilder<
            string, string>(config)
            .Build();       
    }

    public async Task PublishBatchAsync<T>(
        string topic,
        IEnumerable<T> records,
        Func<T, string> keySelector,
        CancellationToken cancellationToken)
    {
        int publishedCount = 0;
        _logger.LogInformation(
            "Publishing messages to Kafka topic {Topic}.",
            topic);

        foreach (T record in records)
        {
            string json =
                JsonSerializer.Serialize(
                    record, JsonOptions
                    );

            Message<string, string> message = new() 
            {
                Key = keySelector(record),
                Value = json
            };

            try
            {
                await _producer.ProduceAsync(
                topic,
                message,
                cancellationToken);

                publishedCount++;
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to publish record with key {Key} to Kafka topic {Topic}. Reason {Reason}",
                    message.Key,
                    topic,
                    ex.Error.Reason);

                throw;
            }
        }
        _logger.LogInformation(
            "Published {PublishedCount} messages to Kafka topic {Topic}",
            publishedCount,
            topic);
    }


    public void Dispose()
    {
        try
        {
            _producer.Flush(TimeSpan.FromSeconds(10));
        }
        finally
        {
            _producer.Dispose();
        }
    }
}
