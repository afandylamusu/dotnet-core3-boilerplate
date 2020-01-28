using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moonlay.MasterData.Domain.DataSets
{
    public interface IDataSetRepository
    {
        Task<List<DataSet>> AllAsync(string domainName);

        Task<List<DataSetAttribute>> GetAttributesAsync(string datasetName, string domainName);

        Task Create(DataSet model, IEnumerable<DataSetAttribute> dataSetAttributes);

        Task Delete(string name);
        Task AddRangeAsync(IEnumerable<DataSet> data);
    }
}
