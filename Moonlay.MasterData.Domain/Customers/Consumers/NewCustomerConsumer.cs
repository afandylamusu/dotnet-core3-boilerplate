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

        protected override int NumMessageToProcess => 1000;

        protected override async Task ConsumeMessages(List<KeyValuePair<MessageHeader, NewCustomerTopic>> messages)
        {
            await _service.CreateBatchAsync(messages.Select(o => new Customer(Guid.NewGuid())
            {
                FirstName = o.Value.FirstName,
                LastName = o.Value.LastName,
                CreatedBy = o.Key.CurrentUser,
                Tested = o.Key.IsCurrentUserDemo,
                UpdatedBy = o.Key.CurrentUser
            }));
        }
    }
}
