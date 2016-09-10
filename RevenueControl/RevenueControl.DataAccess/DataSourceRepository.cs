using RevenueControl.DomainObjects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevenueControl.DomainObjects.Entities;

namespace RevenueControl.DataAccess
{
    public class DataSourceRepository : IDataSourceRepository
    {
        RevenueControlDb _db = new RevenueControlDb();
        public void Dispose()
        {
            _db.Dispose();
        }

        public IEnumerable<DataSource> GetClientDataSources(Client client, string searchTerm = null)
        {
            IEnumerable<DataSource> returnValue;
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                returnValue = _db.DataSources.Where(ds => ds.ClientName == client.Name);
            }
            else
            {
                string toSearch = searchTerm.Trim();
                returnValue = _db.DataSources.Where(ds => ds.ClientName == client.Name && (ds.BankAccount.Contains(toSearch) || (ds.Name != null && ds.Name.Contains(toSearch))));
            }
            return returnValue;
        }

        public DataSource GetDataSource(DataSource dataSource)
        {
            
            return _db.DataSources.Where(ds => ds.ClientName == dataSource.ClientName && ds.Id == dataSource.Id).SingleOrDefault();
        }
    }
}
