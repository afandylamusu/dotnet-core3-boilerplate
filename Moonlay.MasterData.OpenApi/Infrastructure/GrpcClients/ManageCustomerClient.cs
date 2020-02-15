using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moonlay.MasterData.Protos;
using System;
using System.Threading;

namespace Moonlay.MasterData.OpenApi.GrpcClients
{
    internal class ManageCustomerClient : MasterData.Protos.ManageCustomer.ManageCustomerClient, IManageCustomerClient
    {
        private readonly ILogger<ManageCustomerClient> _logger;

        public ManageCustomerClient(ILogger<ManageCustomerClient> logger, ChannelBase channel) : base(channel)
        {
            _logger = logger;
        }

        public override AsyncUnaryCall<AllCustomersReply> AllCustomersAsync(AllCustomersReq request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
        {
            return base.AllCustomersAsync(request, headers, deadline, cancellationToken);
        }

        public override AsyncUnaryCall<Reply> NewCustomerAsync(NewCustomerReq request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
        {
            return base.NewCustomerAsync(request, headers, deadline, cancellationToken);
        }
    }

    public interface IManageCustomerClient
    {
        AsyncUnaryCall<AllCustomersReply> AllCustomersAsync(AllCustomersReq request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);

        AsyncUnaryCall<Reply> NewCustomerAsync(NewCustomerReq request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
    }
}
