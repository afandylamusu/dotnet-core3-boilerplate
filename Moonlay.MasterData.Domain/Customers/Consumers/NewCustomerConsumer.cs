using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.Logging;
using Moonlay.Confluent.Kafka;
using Moonlay.MasterData.Domain.Customers;
using Moonlay.Topics;
using Moonlay.Topics.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moonlay.MasterData.Domain.Customers.Consumers
{
    public interface INewCustomerConsumer : IKafkaConsumer<MessageHeader, NewCustomerTopic> { }

    public class NewCustomerConsumer : KafkaConsumer<MessageHeader, NewCustomerTopic>, INewCustomerConsumer
    {
        private readonly ICustomerUseCase _service;

        public NewCustomerConsumer(ILogger<NewCustomerConsumer> logger, ISchemaRegistryClient schemaRegistryClient, ConsumerConfig config, ICustomerUseCase service) : base(logger, schemaRegistryClient, config)
        {
            _service = service;
        }

        public override string TopicName => "new-customer-topic2";

        protected override async Task ConsumeMessages(ConsumeResult<MessageHeader, NewCustomerTopic> message)
        {
            await _service.NewCustomerAsync(message.Value.FirstName, message.Value.LastName, c =>
            {
                c.CreatedBy = message.Key.CurrentUser;
                c.UpdatedBy = message.Key.CurrentUser;
                c.Tested = message.Key.IsCurrentUserDemo;
            });
        }
    }
}
