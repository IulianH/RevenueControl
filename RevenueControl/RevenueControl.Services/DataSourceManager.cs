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
        IRepository<DataSource> _dsRepo;

        public DataSourceManager(IRepository<DataSource> dsRepo)
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
            IList<DataSource> toReturn;
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                toReturn = _dsRepo.SearchFor(ds => ds.ClientName == client.Name).ToArray();
            }
            else
            {
                string toSearch = searchTerm.Trim();
                toReturn = _dsRepo.SearchFor(ds => ds.ClientName == client.Name && (ds.BankAccount.Contains(toSearch) || (ds.Name != null && ds.Name.Contains(toSearch)))).ToArray();
            }
            return new ActionResponse<DataSource>
            {
                ResultList = toReturn,
                Status = ActionResponseCode.Success
            };

        }
    }
}
