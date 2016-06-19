using RevenueControl.DomainObjects;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.DomainObjects.Exceptions;
using RevenueControl.DomainObjects.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueControl.Services
{
    public class TransactionsManager : ITransactionManager
    {
        private ITransactionRepository transactionRepository;
        private ITransactionFileReader fileReader;
        private Client client;

        CultureInfo culture;

        private static readonly IList<Transaction> EmptyTransactionList = new Transaction[0];
        public TransactionsManager(ITransactionRepository transactionRepository, ITransactionFileReader fileReader, Client client, CultureInfo culture)
        {

            if (transactionRepository == null)
            {
                throw new ArgumentNullException("dataSourceRepository");
            }
            this.transactionRepository = transactionRepository;

            this.fileReader = fileReader;
            if (this.fileReader == null)
            {
                throw new ArgumentNullException("fileReader");
            }
            
            if (client == null || client.ClientId <= 0)
            {
                throw new ArgumentException("client");
            }
            this.client = client;
            this.culture = culture;
        }

        private void ValidateTransactionList(IList<Transaction> transactions, DataSource dataSource)
        {
            if (!transactions.Any())
            {
                throw new InvalidOperationException("A new data source must contain at least 1 transaction");
            }


            foreach (Transaction tr in transactions)
            {
                if (string.IsNullOrWhiteSpace(tr.TransactionDetails) || !GlobalConstants.MaxPeriod.Contains(tr.TransactionDate) || tr.Amount <= 0)
                {
                    throw new InvalidTransactionException();
                }
                tr.DataSource = dataSource;
            }
        }

        public ActionResponse<int> AddTransactionsToDataSource(DataSource dataSource, string transactionReportFile, Period period)
        {
            IList<Transaction> transactionsFromFile;
            if (period == GlobalConstants.MaxPeriod)
            {
                transactionsFromFile = fileReader.Read(transactionReportFile, culture);
            }
            else
            {
                transactionsFromFile = fileReader.Read(transactionReportFile, period, culture);
            }

            Period selectedPeriod = new Period(transactionsFromFile[0].TransactionDate, transactionsFromFile[transactionsFromFile.Count - 1].TransactionDate);

            HashSet<int> indexesToRemove = new HashSet<int>();

           foreach(Transaction dbTransaction in transactionRepository.GetDataSourceTransactions(dataSource, selectedPeriod))
            {
                int idx = transactionsFromFile.IndexOf(dbTransaction);
                if(idx > -1)
                {
                    indexesToRemove.Add(idx);
                }
            }

            transactionRepository.AddTransactionsToDataSource(dataSource, transactionsFromFile.Where((transaction, index) => !indexesToRemove.Contains(index)));

            return new ActionResponse<int>
            {
                Result = transactionsFromFile.Count - indexesToRemove.Count
            };
        }

        public ActionResponse<int> AddTransactionsToDataSource(DataSource dataSource, string transactionReportFile)
        {
            return AddTransactionsToDataSource(dataSource, transactionReportFile, GlobalConstants.MaxPeriod);
        }

        public void Dispose()
        {
            transactionRepository.Dispose();
        }
    }
}
