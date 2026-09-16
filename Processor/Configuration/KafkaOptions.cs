

namespace Processor.Configuration;

public class KafkaOptions
{
    public string BootstrapServers { get; set; } = null!;
    public string GroupId { get; set; } = null!;
}
