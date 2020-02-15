using Microsoft.Extensions.DependencyInjection;
using Moonlay.Confluent.Kafka;
using Moonlay.MasterData.Domain.Customers.Consumers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Moonlay.MasterData.ApiGrpc
{
    public class ConsumerRegister : IConsumerRegister
    {
        public Task[] Consumers(IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            return new Task[] {
                Task.Run(async()=>await serviceProvider.GetRequiredService<INewCustomerConsumer>().Run(stoppingToken)),
                Task.Run(async()=>await serviceProvider.GetRequiredService<IUpdateCustomerConsumer>().Run(stoppingToken))
            };
        }
    }
}
