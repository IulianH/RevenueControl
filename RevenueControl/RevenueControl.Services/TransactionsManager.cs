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

        private ITransactionFileReader fileReader;
        IUnitOfWork unitOfWork;

        public TransactionsManager(IUnitOfWork unitOfWork, ITransactionFileReader fileReader)
        {
            this.unitOfWork = unitOfWork;
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
            return unitOfWork.DataSourceRepository.GetById(dataSource.Id);
        }


        public ParametrizedActionResponse<int> Insert(DataSource dataSource, string transactionReportFile, Period period)
        {
            ParametrizedActionResponse<int> ret = new ParametrizedActionResponse<int>();
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
                    foreach (Transaction dbTransaction in unitOfWork.TransactionRepository.Get(t => t.DataSourceId == dataSource.Id && period.StartDate <= t.TransactionDate && t.TransactionDate <= period.EndDate))
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
                        unitOfWork.TransactionRepository.Insert(transaction);
                    }
                    unitOfWork.Save();
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

        public ParametrizedActionResponse<int> Insert(DataSource dataSource, string transactionReportFile)
        {
            return Insert(dataSource, transactionReportFile, GlobalConstants.MaxPeriod);
        }

        public void Dispose()
        {
            unitOfWork.Dispose();
        }

        public IList<Transaction> Get(DataSource dataSource, string searchTerm = null)
        {
            IList<Transaction> returnValue = unitOfWork.TransactionRepository.Get(tr => tr.DataSourceId == dataSource.Id);
            return returnValue;
        }

        public IList<Transaction> Get(DataSource dataSource, Period period, string searchTerm = null)
        {
            IList<Transaction> returnValue = unitOfWork.TransactionRepository.Get(t => t.DataSourceId == dataSource.Id && period.StartDate >= t.TransactionDate && t.TransactionDate <= period.EndDate);
            return returnValue;
        }
    }
}
