using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moonlay.Confluent.Kafka;
using Moonlay.Core.Models;
using Moonlay.WebApp.Clients;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Moonlay.WebApp
{
    public class CreateDataSetModel : PageModel
    {
        public class NewDataSetForm
        {
            public class AttributeArg
            {
                public string Name { get; set; }
            }

            [Required]
            [MaxLength(64)]
            [Display(Name="Domain")]
            public string DomainName { get; set; }

            [Required]
            [MaxLength(64)]
            public string Name { get; set; }

            [Required]
            [MaxLength(64)]
            [Display(Name = "Organization")]
            public string OrgName { get; set; }

            public List<AttributeArg> Attributes { get; set; } = new List<AttributeArg>();
        }

        private readonly IKafkaProducer _producer;
        private readonly IManageDataSetClient _dataSetClient;
        private readonly ISignInService _signIn;

        [BindProperty]
        public NewDataSetForm Form { get; set; }

        public CreateDataSetModel(IManageDataSetClient dataSetClient, ISignInService signIn)
        {
            _dataSetClient = dataSetClient;
            _signIn = signIn;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var request = new MasterData.Protos.NewDatasetReq
            {
                Name = Form.Name,
                DomainName = Form.DomainName,
                OrganizationName = Form.OrgName
            };

            Form.Attributes.ForEach(o => request.Attributes.Add(new MasterData.Protos.AttributeArg { Name = o.Name }));

            var reply = await _dataSetClient.NewDatasetAsync(request);

            return RedirectToPage("./Index");
        }
    }
}