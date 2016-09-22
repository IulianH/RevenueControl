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
        ActionResponse Insert(DataSource dataSource);

        IList<DataSource> Get(string clientName, string searchTerm = null);

        bool HasTransactions(DataSource dataSource);

        DataSource GetById(int id, string clientName);

        ActionResponse Delete(DataSource dataSource);

        ActionResponse Update(DataSource dataSource);
    }
}
