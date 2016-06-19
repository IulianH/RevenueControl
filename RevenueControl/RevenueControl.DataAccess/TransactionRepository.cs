using RevenueControl.DomainObjects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevenueControl.DomainObjects.Entities;

namespace RevenueControl.DataAccess
{
    public class TransactionRepository : ITransactionRepository
    {
        public void AddTransactionsToDataSource(DataSource dataSource, IEnumerable<Transaction> transactions)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Transaction> GetDataSourceTransactions(DataSource dataSource)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Transaction> GetDataSourceTransactions(DataSource dataSource, Period period)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Transaction> Search(DataSource dataSource, Transaction patternTransaction, Period period)
        {
            throw new NotImplementedException();
        }
    }
}
