using RevenueControl.DomainObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueControl.DomainObjects.Interfaces
{
    public interface IDataSourceManager : IDisposable
    {
        ActionResponse<DataSource> CreateDataSource(DataSource dataSource);

        ActionResponse<DataSource> GetClientDataSources(Client client, string searchTerm = null);

        bool HasTransactions(DataSource dataSource);
    }
}
