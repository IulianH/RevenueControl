using RevenueControl.DomainObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueControl.DomainObjects.Interfaces
{
    public interface ITransactionManager : IDisposable
    {
        ParametrizedActionResponse<int> Insert(DataSource dataSource, string transactionReportFile);

        ParametrizedActionResponse<int> Insert(DataSource dataSource, string transactionReportFile, Period period);

        IList<Transaction> Get(DataSource dataSource, string searchTerm = null);

        IList<Transaction> Get(DataSource dataSource, Period period, string searchTerm = null);

        ActionResponse TagTransaction(int transactionId, IList<string> tags);
    }
}
