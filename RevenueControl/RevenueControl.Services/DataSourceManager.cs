using RevenueControl.DomainObjects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevenueControl.DomainObjects;
using RevenueControl.DomainObjects.Entities;

namespace RevenueControl.Services
{
    public class DataSourceManager : IDataSourceManager
    {
        IDataSourceRepository _dsRepo;
        Client _client;

        public DataSourceManager(IDataSourceRepository dsRepo, Client client)
        {
            _dsRepo = dsRepo;
            _client = client; 
        }

        public ActionResponse<DataSource> CreateDataSource(DataSource dataSource)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _dsRepo.Dispose();
        }

        public ActionResponse<DataSource> GetDataSources()
        {
            ICollection<DataSource> toReturn = _dsRepo.GetClientDataSources(_client).ToList();
            return new ActionResponse<DataSource>
            {
                ResultList = toReturn,
                Status = ActionResponseCode.Success
            };

        }
    }
}
