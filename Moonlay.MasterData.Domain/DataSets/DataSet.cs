using Moonlay.Core.Models;

namespace Moonlay.MasterData.Domain.DataSets
{
    public class DataSet : IModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DomainName { get; set; }
    }
}
