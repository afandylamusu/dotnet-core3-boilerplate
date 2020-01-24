using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.Logging;
using Moonlay.Confluent.Kafka;
using Moonlay.Topics;
using Moonlay.Topics.MDM.DataSets;
using System.Threading.Tasks;

namespace Moonlay.MasterData.Domain.DataSets.Consumers
{
    public interface INewDataSetConsumer : IKafkaConsumer<MessageHeader, NewDataSetTopic> { }

    public class NewDataSetConsumer : KafkaConsumer<MessageHeader, NewDataSetTopic>, INewDataSetConsumer
    {
        private readonly IDataSetService _dataSetService;

        public NewDataSetConsumer(ILogger<NewDataSetConsumer> logger, ISchemaRegistryClient schemaRegistryClient, ConsumerConfig config, IDataSetService dataSetService) : base(logger, schemaRegistryClient, config)
        {
            _dataSetService = dataSetService;
        }

        public override string TopicName => "mdm-newdataset-topic";

        protected override async Task ConsumeMessage(ConsumeResult<MessageHeader, NewDataSetTopic> consumeResult)
        {
            await _dataSetService.NewDataSet(consumeResult.Value.Name, consumeResult.Value.DomainName, consumeResult.Value.OrgName, new DataSetAttribute[] { });
        }
    }
}
