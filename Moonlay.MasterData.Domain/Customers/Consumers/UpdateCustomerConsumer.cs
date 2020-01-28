using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.Logging;
using Moonlay.Confluent.Kafka;
using Moonlay.Topics;
using Moonlay.Topics.Customers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moonlay.MasterData.Domain.Customers.Consumers
{
    public interface IUpdateCustomerConsumer : IKafkaConsumer<MessageHeader, UpdateCustomerTopic> { }

    public class UpdateCustomerConsumer : KafkaConsumer<MessageHeader, UpdateCustomerTopic>, IUpdateCustomerConsumer
    {
        public override string TopicName => "update-customer-topic";

        private readonly ICustomerUseCase _service;

        public UpdateCustomerConsumer(ILogger<UpdateCustomerConsumer> logger, ISchemaRegistryClient schemaRegistryClient, ConsumerConfig config, ICustomerUseCase service) : base(logger, schemaRegistryClient, config)
        {
            _service = service;
        }

        protected override Task ConsumeMessages(List<KeyValuePair<MessageHeader, UpdateCustomerTopic>> messages)
        {
            throw new NotImplementedException();
        }
    }
}
