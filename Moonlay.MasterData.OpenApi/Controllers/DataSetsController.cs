using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moonlay.MasterData.OpenApi.Clients;
using Moonlay.MasterData.OpenApi.Controllers.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Moonlay.MasterData.OpenApi.Controllers
{
    [ApiController]
    [Route("api/datasets")]
    public class DataSetsController : ControllerBase
    {
        private readonly ILogger<DataSetsController> _logger;
        private readonly IManageDataSetClient _manageDataSetClient;

        public DataSetsController(ILogger<DataSetsController> logger, IManageDataSetClient manageDataSetClient)
        {
            _logger = logger;
            _manageDataSetClient = manageDataSetClient;
        }

        [HttpGet()]
        public async Task<ActionResult<List<DataSetDto>>> Get()
        {
            var data = await _manageDataSetClient.AllDataSetsAsync(new Protos.AllDataSetsReq { DomainName = "public" });

            return await Task.FromResult(Ok(data.Data.Select(o => new DataSetDto(o)).ToList()));
        }
    }
}
