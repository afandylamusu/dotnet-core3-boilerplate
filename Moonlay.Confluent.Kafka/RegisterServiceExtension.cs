using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Moonlay.Confluent.Kafka
{
    public static class RegisterServiceExtension
    {
        static void AddKafkaSchemaRegistry(this IServiceCollection services)
        {
            services.AddSingleton(c =>
            {
                var configuration = c.GetService<IConfiguration>();
                var values = configuration.GetSection("Kafka:SchemaRegistry").GetChildren().ToDictionary(x => x.Key, x => x.Value);
                var config = new SchemaRegistryConfig();
                foreach (var val in values)
                    config.Set(val.Key, val.Value);

                return config;
            });

            services.AddSingleton<ISchemaRegistryClient>(c => new CachedSchemaRegistryClient(c.GetRequiredService<SchemaRegistryConfig>()));
        }

        public static void AddKafkaProducer(this IServiceCollection services)
        {
            services.AddKafkaSchemaRegistry();

            services.AddSingleton(c =>
            {
                var configuration = c.GetService<IConfiguration>();
                var values = configuration.GetSection("Kafka:Producer").GetChildren().ToDictionary(x => x.Key, x => x.Value);
                return new ProducerConfig(values);
            });

            services.AddScoped<IKafkaProducer, KafkaProducer>();
        }

        public static void AddKafkaConsumer(this IServiceCollection services)
        {
            services.AddKafkaSchemaRegistry();
            services.AddSingleton(c =>
            {
                var configuration = c.GetService<IConfiguration>();
                var values = configuration.GetSection("Kafka:Consumer").GetChildren().ToDictionary(x => x.Key, x => x.Value);
                return new ConsumerConfig(values);
            });

            services.AddHostedService<HostedConsumers>();
        }
    }
}
