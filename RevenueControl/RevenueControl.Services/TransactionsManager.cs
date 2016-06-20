using RevenueControl.DomainObjects;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.DomainObjects.Exceptions;
using RevenueControl.DomainObjects.Interfaces;
using RevenueControl.Resources;
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
        private IDataSourceRepository dataSourceRepository;
        private ITransactionRepository transactionRepository;
        private ITransactionFileReader fileReader;
        private Client client;

        CultureInfo culture;

        public TransactionsManager(IDataSourceRepository dataSourceRepository, 
            ITransactionRepository transactionRepository, 
            ITransactionFileReader fileReader, Client client, CultureInfo culture)
        {
            this.dataSourceRepository = dataSourceRepository;
            this.transactionRepository = transactionRepository;
            this.fileReader = fileReader;
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

        private DataSource ValidateRequest(DataSource dataSource)
        {
            DataSource dbDataSource = dataSourceRepository.GetById(dataSource.Id);
            if(dbDataSource != null)
            {
                if(dbDataSource.ClientId != client.Id)
                {
                    dbDataSource = null;
                }
            }
            return dbDataSource;
        }

        public ActionResponse<int> AddTransactionsToDataSource(DataSource dataSource, string transactionReportFile, Period period)
        {
            ActionResponse<int> ret = new ActionResponse<int>();
            DataSource repoDataSource = ValidateRequest(dataSource);
            if (repoDataSource != null)
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

                if (transactionsFromFile.Count > 0)
                {
                    Period selectedPeriod = new Period(transactionsFromFile[0].TransactionDate, transactionsFromFile[transactionsFromFile.Count - 1].TransactionDate);
                    HashSet<int> indexesToRemove = new HashSet<int>();
                    foreach (Transaction dbTransaction in transactionRepository.GetDataSourceTransactions(repoDataSource, selectedPeriod))
                    {
                        int idx = transactionsFromFile.IndexOf(dbTransaction);
                        if (idx > -1)
                        {
                            indexesToRemove.Add(idx);
                        }
                    }
                    transactionRepository.AddTransactionsToDataSource(repoDataSource, transactionsFromFile.Where((transaction, index) => !indexesToRemove.Contains(index)));
                    ret.Result = transactionsFromFile.Count - indexesToRemove.Count;
                }
                else
                {
                    ret.Status = ActionResponseCode.NoActionPerformed;
                    ret.ActionResponseMessage = Localization.GetZeroTransactionsInFile(culture);
                }
            }
            else
            {
                ret.Result = -1;
                ret.Status = ActionResponseCode.InvalidInput;
            }
            return ret;
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
