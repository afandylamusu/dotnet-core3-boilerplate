using Moonlay.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moonlay.MasterData.Domain.DataSets
{
    public class DomainSchema : IModel
    {
        public string Name { get; set; }
        public string OrgName { get; set; }

    }
}
