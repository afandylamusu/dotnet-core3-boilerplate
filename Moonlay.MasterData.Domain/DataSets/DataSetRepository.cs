using Dapper;
using Moonlay.Core.Models;
using Moonlay.MasterData.Domain.DataSets.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moonlay.MasterData.Domain.DataSets
{
    public class DataSetRepository : IDataSetRepository
    {
        private readonly IDbxConnection _db;

        public DataSetRepository(IDbxConnection db)
        {
            _db = db;
        }

        public Task<List<DataSet>> AllAsync(string domainName)
        {
            var datasets = _db.Connection.Query<InformationTable>("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE table_schema=@domain",
                new { domain = domainName }).Select(o => new DataSet
                {
                    Name = o.TABLE_NAME,
                    Description = string.Empty,
                    DomainName = o.TABLE_SCHEMA
                }).ToList();

            return Task.FromResult(datasets);

        }

        public Task Create(DataSet model, IEnumerable<DataSetAttribute> dataSetAttributes)
        {
            throw new NotImplementedException();
        }

        public Task Delete(string name)
        {
            throw new NotImplementedException();
        }

        public Task<List<DataSetAttribute>> GetAttributesAsync(string datasetName, string domainName)
        {
            var datasets = _db.Connection.Query<InformationTable>("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=@domain and TABLE_NAME=@dataset",
               new { domain = domainName, dataset = datasetName }).Select(o => new DataSetAttribute
               {
                   Name = o.COLUMN_NAME,
                   Type = o.DATA_TYPE,
                   DataSetName = o.TABLE_NAME,
                   DomainName = o.TABLE_SCHEMA
               }).ToList();

            return Task.FromResult(datasets);
        }
    }
}
