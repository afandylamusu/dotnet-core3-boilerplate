using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moonlay.MasterData.OpenApi.GrpcClients;
using Moonlay.MasterData.OpenApi.Controllers.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moonlay.MasterData.OpenApi.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly ILogger<CustomersController> _logger;
        private readonly IManageCustomerClient _manageCustomerClient;

        public CustomersController(ILogger<CustomersController> logger, IManageCustomerClient manageCustomerClient)
        {
            _logger = logger;
            _manageCustomerClient = manageCustomerClient;
        }

        [HttpGet()]
        public async Task<ActionResult<List<CustomerDto>>> Get(int page = 0, int pageSize = 25)
        {
            var reply = await _manageCustomerClient.AllCustomersAsync(new Protos.AllCustomersReq { Page = page, PageSize = pageSize });
            if (!reply.Success)
                return BadRequest(new { error = true, message = reply.Message });

            return Ok(reply.Data.Select(o => new CustomerDto(o)).ToList());
        }

        [HttpPost()]
        public async Task<ActionResult> Post(NewCustomerForm form)
        {
            if (ModelState.IsValid)
            {
                var reply = await _manageCustomerClient.NewCustomerAsync(new Protos.NewCustomerReq { FirstName = form.FirstName, LastName = form.LastName });
                if (!reply.Success)
                    return BadRequest(new { error = true, message = reply.Message });
            }
            else
                return BadRequest();

            return Ok();
        }

    }
}