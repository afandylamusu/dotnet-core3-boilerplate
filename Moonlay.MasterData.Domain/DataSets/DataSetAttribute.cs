using Moonlay.Core.Models;

namespace Moonlay.MasterData.Domain.DataSets
{
    public class DataSetAttribute : IModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string DataSetName { get; set; }
        public string DomainName { get; set; }
    }
}
