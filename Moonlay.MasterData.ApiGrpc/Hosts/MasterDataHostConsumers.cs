using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moonlay.Confluent.Kafka;
using Moonlay.MasterData.Domain.Customers.Consumers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Moonlay.MasterData.ApiGrpc.Hosts
{
    public class MasterDataHostConsumers : HostedConsumers
    {
        public MasterDataHostConsumers(IServiceProvider services, ILogger<MasterDataHostConsumers> logger) : base(services, logger)
        {

        }

        protected override Task[] GetConsumers(IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            return new Task[] {
                Task.Run(async()=>await serviceProvider.GetRequiredService<INewCustomerConsumer>().Run(stoppingToken)),
                Task.Run(async()=>await serviceProvider.GetRequiredService<IUpdateCustomerConsumer>().Run(stoppingToken))
            };
        }
    }
}
