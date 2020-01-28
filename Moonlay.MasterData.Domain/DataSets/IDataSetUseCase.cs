using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moonlay.MasterData.Domain.DataSets
{
    public interface IDataSetUseCase
    {
        Task NewDataSet(string name, string domainName, string orgName, IEnumerable<DataSetAttribute> attributes, Action<DataSet> beforeSave = null);
        Task Remove(string name);
        Task<List<DataSet>> AllDataSets(string domainName);
        Task CreateBatchAsync(IEnumerable<DataSet> data);
    }

    public class DataSetUseCase : IDataSetUseCase
    {
        private readonly IDataSetRepository _dataSetRepository;

        public DataSetUseCase(IDataSetRepository dataSetRepository)
        {
            _dataSetRepository = dataSetRepository;
        }

        public async Task<List<DataSet>> AllDataSets(string domainName)
        {
            return await _dataSetRepository.AllAsync(domainName);
        }

        public async Task CreateBatchAsync(IEnumerable<DataSet> data)
        {
            await _dataSetRepository.AddRangeAsync(data);
        }

        public async Task NewDataSet(string name, string domainName, string orgName, IEnumerable<DataSetAttribute> attributes, Action<DataSet> beforeSave = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("message", nameof(name));
            }

            if (string.IsNullOrEmpty(domainName))
            {
                throw new ArgumentException("message", nameof(domainName));
            }

            if (string.IsNullOrEmpty(orgName))
            {
                throw new ArgumentException("message", nameof(orgName));
            }

            await _dataSetRepository.Create(new DataSet { Description = "", Name = name, DomainName = domainName }, attributes);
        }

        public Task Remove(string name)
        {
            throw new NotImplementedException();
        }
    }
}
