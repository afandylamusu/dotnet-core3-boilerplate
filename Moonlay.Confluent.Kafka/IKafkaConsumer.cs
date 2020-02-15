using Confluent.Kafka;
using System.Threading;
using System.Threading.Tasks;

namespace Moonlay.Confluent.Kafka
{
    public interface IKafkaConsumer
    {
        Task Run(CancellationToken cancellationToken = default);
        
        string TopicName { get; }
    }

    public interface IKafkaConsumer<TKey, TValue> : IKafkaConsumer
    {
        IConsumer<TKey, TValue> Consumer { get; }
    }
}
