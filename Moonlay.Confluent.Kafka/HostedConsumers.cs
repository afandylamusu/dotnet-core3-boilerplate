
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Moonlay.Confluent.Kafka
{
    public class HostedConsumers : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        public HostedConsumers(IServiceProvider services,
            ILogger<HostedConsumers> logger)
        {
            Services = services;
            _logger = logger;
        }

        public IServiceProvider Services { get; }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Kafka Consumers Hosted Service is running.");

            Task.Run(async () => await DoConsumers(stoppingToken), stoppingToken);

            return Task.CompletedTask;
        }

        private Task DoConsumers(CancellationToken stoppingToken)
        {
            using (var scope = Services.CreateScope())
            {
                var consumerRegistrations = scope.ServiceProvider.GetRequiredService<IConsumerRegister>();
                Task.WaitAll(consumerRegistrations.Consumers(scope.ServiceProvider, stoppingToken));
            }

            return Task.CompletedTask;
        }

        //protected abstract Task[] GetConsumers(IServiceProvider serviceProvider, CancellationToken stoppingToken);

        public async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Kafka Consumers Hosted Service is stopping.");

            await Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }

}