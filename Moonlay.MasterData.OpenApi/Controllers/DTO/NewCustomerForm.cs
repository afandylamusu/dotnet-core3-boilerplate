using System.ComponentModel.DataAnnotations;

namespace Moonlay.MasterData.OpenApi.Controllers
{
    public class NewCustomerForm
    {
        [Required]
        [MaxLength(64)]
        public string FirstName { get; set; }

        [MaxLength(64)]
        public string LastName { get; set; }
    }
}