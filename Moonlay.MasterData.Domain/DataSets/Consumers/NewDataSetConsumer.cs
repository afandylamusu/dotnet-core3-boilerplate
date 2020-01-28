using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.Logging;
using Moonlay.Confluent.Kafka;
using Moonlay.Topics;
using Moonlay.Topics.MDM.DataSets;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moonlay.MasterData.Domain.DataSets.Consumers
{
    public interface INewDataSetConsumer : IKafkaConsumer<MessageHeader, NewDataSetTopic> { }

    public class NewDataSetConsumer : KafkaConsumer<MessageHeader, NewDataSetTopic>, INewDataSetConsumer
    {
        private readonly IDataSetUseCase _dataSetService;

        public NewDataSetConsumer(ILogger<NewDataSetConsumer> logger, ISchemaRegistryClient schemaRegistryClient, ConsumerConfig config, IDataSetUseCase dataSetService) : base(logger, schemaRegistryClient, config)
        {
            _dataSetService = dataSetService;
        }

        public override string TopicName => "mdm-newdataset-topic";

        protected override int NumMessageToProcess => 100;

        protected override async Task ConsumeMessages(List<KeyValuePair<MessageHeader, NewDataSetTopic>> messages)
        {
            await _dataSetService.CreateBatchAsync(messages.Select(o => new DataSet { }));
        }
    }
}
