using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Moonlay.Confluent.Kafka;
using Moonlay.Topics;
using Moonlay.Topics.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moonlay.DemoKafkaClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ProducerConfig() { BootstrapServers = "192.168.99.109:9092" };
            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = "192.168.99.109:8081",
                // Note: you can specify more than one schema registry url using the
                // schema.registry.url property for redundancy (comma separated list). 
                // The property name is not plural to follow the convention set by
                // the Java implementation.
                // optional schema registry client properties:
                RequestTimeoutMs = 5000,
                MaxCachedSchemas = 10
            };

            // If serializers are not specified, default serializers from
            // `Confluent.Kafka.Serializers` will be automatically used where
            // available. Note: by default strings are encoded as UTF8.
            using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);

            using var p = new ProducerBuilder<MessageHeader, NewCustomerTopic>(config)
                .SetKeySerializer(new AvroSerializer<MessageHeader>(schemaRegistry))
                .SetValueSerializer(new AvroSerializer<NewCustomerTopic>(schemaRegistry))
                .Build();

            try
            {
                Parallel.ForEach(Enumerable.Range(1, 10000), i => {
                    var dr = p.ProduceAsync("new-customer-topic2", new Message<MessageHeader, NewCustomerTopic>
                    {
                        Key = new MessageHeader
                        {
                            AppOrigin = "DemoKafkaClient",
                            CurrentUser = "demo",
                            Timestamp = DateTime.Now.ToString("s"),
                            Token = Guid.NewGuid().ToString()
                        },
                        Value = new NewCustomerTopic
                        {
                            FirstName = "Customer "+i,
                            LastName = "Asyik"
                        }
                    }).Result;

                    Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                });
               

                p.Flush();

            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            }
        }
    }
}
