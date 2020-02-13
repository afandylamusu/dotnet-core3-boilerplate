using System;
using System.Threading;
using System.Threading.Tasks;

namespace Moonlay.Confluent.Kafka
{
    public interface IConsumerRegister
    {
        Task[] Consumers(IServiceProvider serviceProvider, CancellationToken stoppingToken);
    }
}