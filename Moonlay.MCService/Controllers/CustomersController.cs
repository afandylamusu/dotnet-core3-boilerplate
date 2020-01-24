using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moonlay.MasterData.WebApi.Controllers.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moonlay.MasterData.WebApi.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ILogger<CustomersController> logger)
        {
            _logger = logger;
        }

        [HttpGet()]
        public async Task<ActionResult<List<CustomerDto>>> Get()
        {
            return await Task.FromResult(Ok(new List<CustomerDto>()));
        }

    }
}