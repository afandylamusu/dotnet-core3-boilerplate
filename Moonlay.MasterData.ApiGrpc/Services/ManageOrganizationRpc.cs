using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moonlay.MasterData.Protos;
using System.Threading.Tasks;

namespace Moonlay.MasterData.ApiGrpc.Services
{
    public class ManageOrganizationRpc : Protos.ManageOrganization.ManageOrganizationBase
    {
        private readonly ILogger<ManageOrganizationRpc> _logger;
        public ManageOrganizationRpc(ILogger<ManageOrganizationRpc> logger)
        {
            _logger = logger;
        }

        public override Task<Reply> NewOrganization(NewOrganizationReq request, ServerCallContext context)
        {
            return Task.FromResult(new Reply
            {
                Success = true,
                Message = "Successfully"
            });
        }
    }
}
