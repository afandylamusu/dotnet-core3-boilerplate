using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moonlay.MasterData.Domain.Customers;
using Moonlay.MasterData.Protos;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Moonlay.MasterData.ApiGrpc.Services
{
    public class ManageCustomerRpc : Protos.ManageCustomer.ManageCustomerBase
    {
        private readonly ILogger<ManageCustomerRpc> _logger;
        private readonly ICustomerUseCase _customerService;

        public ManageCustomerRpc(ILogger<ManageCustomerRpc> logger, ICustomerUseCase customerService)
        {
            _logger = logger;
            _customerService = customerService;
        }

        public override async Task<Reply> NewCustomer(NewCustomerReq request, ServerCallContext context)
        {
            var reply = new Reply { Success = true };
            try
            {
                await _customerService.NewCustomerAsync(request.FirstName, request.LastName);
            }
            catch(Exception ex)
            {
                reply.Success = false;
                reply.Message = $"{GetExeptionMessage(ex)}";
            }

            return reply;
        }

        private string GetExeptionMessage(Exception ex, string currentMessage = "")
        {
            currentMessage += (" - " + ex.Message);
            
            if (ex.InnerException != null)
                return GetExeptionMessage(ex.InnerException, currentMessage);
            else
                return currentMessage;
        }

        public override async Task<AllCustomersReply> AllCustomers(AllCustomersReq request, ServerCallContext context)
        {
            var reply = new AllCustomersReply { Success = true };

            try
            {
                var data = await _customerService.SearchAsync(c => true, request.Page, request.PageSize);
                
                data.ForEach(o => reply.Data.Add(new CustomerArg
                {
                    Id = o.Id.ToString(),
                    FirstName = o.FirstName,
                    LastName = o.LastName,
                    CreatedBy = o.CreatedBy,
                    CreatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTimeOffset(o.CreatedAt),
                    UpdatedBy = o.UpdatedBy,
                    UpdatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTimeOffset(o.UpdatedAt)
                }));
            }
            catch(Exception ex)
            {
                reply.Success = false;
                reply.Message = ex.Message;
            }

            return reply;
        }
    }
}
