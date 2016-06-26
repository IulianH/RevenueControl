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

        public DataSource GetById(int dataSourceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataSource> GetClientDataSources(Client client)
        {
            return _db.DataSources.Where(ds => ds.ClientId == client.Id);
        }
    }
}
