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
    
        int AddTransactionsToDataSource(DataSource dataSource, string transactionReportFile);

        int AddTransactionsToDataSource(DataSource dataSource, string transactionReportFile, Period period);
    }
}
