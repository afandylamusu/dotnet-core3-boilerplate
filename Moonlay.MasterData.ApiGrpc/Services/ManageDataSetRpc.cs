using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moonlay.MasterData.Domain.DataSets;
using Moonlay.MasterData.Protos;
using System.Linq;
using System.Threading.Tasks;

namespace Moonlay.MasterData.ApiGrpc.Services
{
    public class ManageDataSetRpc : Protos.ManageDataSet.ManageDataSetBase
    {
        private readonly ILogger<ManageDataSetRpc> _logger;
        private readonly IDataSetService _dataSetService;

        public ManageDataSetRpc(ILogger<ManageDataSetRpc> logger, IDataSetService dataSetService)
        {
            _logger = logger;
            _dataSetService = dataSetService;
        }

        public override async Task<Reply> NewDataset(NewDatasetReq request, ServerCallContext context)
        {
            await _dataSetService.NewDataSet(request.Name, request.DomainName, request.OrganizationName, request.Attributes.Select(o => new DataSetAttribute
            {
                DataSetName = request.Name,
                DomainName = request.DomainName,
                Name = o.Name,
                Type = o.Type
            }));

            return new Reply
            {
                Success = true,
                Message = "Successfully"
            };
        }

        public override async Task<Reply> RemoveDataSet(RemoveDataSetReq request, ServerCallContext context)
        {
            await _dataSetService.Remove(request.Name);

            return new Reply
            {
                Success = true,
                Message = "Successfully"
            };
        }

        public override async Task<AllDataSetsReply> AllDataSets(AllDataSetsReq request, ServerCallContext context)
        {
            var reply = new AllDataSetsReply
            {
                Success = true,
            };

            (await _dataSetService.AllDataSets(request.DomainName)).ForEach(i => {
                reply.Data.Add(new DataSetArg { Name = i.Name });
            });

            return reply;
        }
    }
}
