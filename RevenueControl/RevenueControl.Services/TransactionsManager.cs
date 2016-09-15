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
        private IRepository<DataSource> dataSourceRepository;
        private IRepository<Transaction> transactionRepository;
        private ITransactionFileReader fileReader;


        public TransactionsManager(IRepository<DataSource> dataSourceRepository, 
            IRepository<Transaction> transactionRepository, 
            ITransactionFileReader fileReader)
        {
            this.dataSourceRepository = dataSourceRepository;
            this.transactionRepository = transactionRepository;
            this.fileReader = fileReader;
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
                tr.DataSourceId = dataSource.Id;
            }
        }

        private DataSource GetDataSource(DataSource dataSource)
        {
            return dataSourceRepository.GetById(dataSource.Id);
        }


        public ActionResponse<int> AddTransactionsToDataSource(DataSource dataSource, string transactionReportFile, Period period)
        {
            ActionResponse<int> ret = new ActionResponse<int>();
            DataSource repoDataSource = GetDataSource(dataSource);

            if (repoDataSource != null)
            {
                CultureInfo culture = new CultureInfo(repoDataSource.Culture);
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
                    foreach (Transaction dbTransaction in transactionRepository.SearchFor(t => t.DataSourceId == dataSource.Id && period.StartDate >= t.TransactionDate && t.TransactionDate <= period.EndDate))
                    {
                        int idx = transactionsFromFile.IndexOf(dbTransaction);
                        if (idx > -1)
                        {
                            indexesToRemove.Add(idx);
                        }
                    }
                    foreach(Transaction transaction in transactionsFromFile.Where((transaction, index) => !indexesToRemove.Contains(index)))
                    {
                        transaction.DataSourceId = repoDataSource.Id;
                        transactionRepository.Insert(transaction);
                    }

                    ret.Result = transactionsFromFile.Count - indexesToRemove.Count;
                    ret.Status = ActionResponseCode.Success;
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
                ret.Status = ActionResponseCode.NotFound;
            }
            return ret;
        }

        public ActionResponse<int> AddTransactionsToDataSource(DataSource dataSource, string transactionReportFile)
        {
            return AddTransactionsToDataSource(dataSource, transactionReportFile, GlobalConstants.MaxPeriod);
        }

        public void Dispose()
        {
            if (transactionRepository != null)
            {
                transactionRepository.Dispose();
            }
            if(dataSourceRepository != null)
            {
                dataSourceRepository.Dispose();
            }
        }

        public ActionResponse<Transaction> GetDataSourceTransactions(DataSource dataSource, string searchTerm = null)
        {

            return new ActionResponse<Transaction>
            {
                Status = ActionResponseCode.Success,
                ResultList = transactionRepository.SearchFor(tr => tr.DataSourceId == dataSource.Id).ToArray()
            };

        }

        public ActionResponse<Transaction> GetDataSourceTransactions(DataSource dataSource, Period period, string searchTerm = null)
        {
            return new ActionResponse<Transaction>
            {
                Status = ActionResponseCode.Success,
                ResultList = transactionRepository.SearchFor(t => t.DataSourceId == dataSource.Id && period.StartDate >= t.TransactionDate && t.TransactionDate <= period.EndDate).ToArray()
            };
        }
    }
}
