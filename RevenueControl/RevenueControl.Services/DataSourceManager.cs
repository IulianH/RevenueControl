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

        public DataSourceManager(IDataSourceRepository dsRepo)
        {
            _dsRepo = dsRepo;
        }

        public ActionResponse<DataSource> CreateDataSource(DataSource dataSource)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (_dsRepo != null)
            {
                _dsRepo.Dispose();
            }
        }

        public ActionResponse<DataSource> GetClientDataSources(Client client, string searchTerm = null)
        {
            IList<DataSource> toReturn = _dsRepo.GetClientDataSources(client, searchTerm).ToList();
            return new ActionResponse<DataSource>
            {
                ResultList = toReturn,
                Status = ActionResponseCode.Success
            };

        }

        public ActionResponse<DataSource> GetDataSources(Client client)
        {
            throw new NotImplementedException();
        }
    }
}
