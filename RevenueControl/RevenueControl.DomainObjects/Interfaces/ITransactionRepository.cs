using RevenueControl.DomainObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueControl.DomainObjects.Interfaces
{
    public interface ITransactionRepository : IDisposable
    {
        IEnumerable<Transaction> GetDataSourceTransactions(DataSource dataSource);

        IEnumerable<Transaction> GetDataSourceTransactions(DataSource dataSource, Period period);

        void AddTransactionsToDataSource(DataSource dataSource, IEnumerable<Transaction> transactions);

        IEnumerable<Transaction> Search(DataSource dataSource, Transaction patternTransaction, Period period);
    }
}
