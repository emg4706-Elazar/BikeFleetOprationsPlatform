

namespace Producer.Services;

public interface IKafkaProducerService
{
    Task PublishBatchAsync<T>(
        string topic,
        IEnumerable<T> records,
        Func<T, string> keySelector,
        CancellationToken cancellationToken);
}
