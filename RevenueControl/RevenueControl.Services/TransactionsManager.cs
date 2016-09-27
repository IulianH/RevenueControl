using RevenueControl.DomainObjects;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.DomainObjects.Exceptions;
using RevenueControl.DomainObjects.Interfaces;
using RevenueControl.Resource;
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

        public TransactionsManager(IUnitOfWork unitOfWork) : this(unitOfWork, null) { }
        

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

                //removing any duplicates
                HashSet<Transaction> transactionHash = new HashSet<Transaction>();
                foreach(Transaction transaction in transactionsFromFile)
                {
                    while (transactionHash.Contains(transaction))
                    {
                        transaction.TransactionDate = transaction.TransactionDate.AddSeconds(1);
                    }
                    transactionHash.Add(transaction);
                }
                

                if (transactionsFromFile.Count > 0)
                {
                    Period selectedPeriod = new Period(transactionsFromFile[0].TransactionDate, transactionsFromFile[transactionsFromFile.Count - 1].TransactionDate);
                    IList<Transaction> dbTransactions = unitOfWork.TransactionRepository.Set.
                        Where(t => t.DataSourceId == dataSource.Id && period.StartDate <= t.TransactionDate && t.TransactionDate <= period.EndDate).ToList();
                    foreach (Transaction dbTransaction in dbTransactions)
                    {
                        if(transactionHash.Contains(dbTransaction))
                        {
                            transactionHash.Remove(dbTransaction);
                        }
                    }
                    foreach(Transaction transaction in transactionsFromFile.Where(transaction => transactionHash.Contains(transaction)))
                    {
                        transaction.DataSourceId = repoDataSource.Id;
                        unitOfWork.TransactionRepository.Insert(transaction);
                    }
                    unitOfWork.Save();
                    ret.Result = transactionHash.Count;
                    ret.Status = ActionResponseCode.Success;
                }
                else
                {
                    ret.Status = ActionResponseCode.NoActionPerformed;
                    ret.ActionResponseMessage = Resources.ZeroTransactionsInFile;
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
            IList<Transaction> returnValue = unitOfWork.TransactionRepository.Set.Where(tr => tr.DataSourceId == dataSource.Id).ToList();
            foreach(Transaction transaction in returnValue)
            {
                transaction.Tags = unitOfWork.TransactionTagRepository.Set.Where(tag => tag.TransactionId == transaction.Id).Select(tag => tag.Tag).ToArray();           
            }
            return returnValue;
        }

        public IList<Transaction> Get(DataSource dataSource, Period period, string searchTerm = null)
        {
            IList<Transaction> returnValue = unitOfWork.TransactionRepository.Set.
                Where(t => t.DataSourceId == dataSource.Id && period.StartDate >= t.TransactionDate && t.TransactionDate <= period.EndDate).ToArray();
            return returnValue;
        }

        public ActionResponse TagTransaction(int transactionId, IList<string> tags)
        {
            IList<string> existingTags = unitOfWork.TransactionTagRepository.Set.Where(tag => tag.TransactionId == transactionId).Select(tag => tag.Tag.ToUpper()).ToArray();
            IList<string> toAddTags = tags.Where(tag => !string.IsNullOrWhiteSpace(tag) && !existingTags.Contains(tag.ToUpper())).Select(tag => tag.Trim()).ToArray();
            if (toAddTags.Count > 0)
            { 
                DataSource dataSource = unitOfWork.DataSourceRepository.GetById(unitOfWork.TransactionRepository.GetById(transactionId).DataSourceId);
                foreach(string tag in toAddTags)
                {
                    unitOfWork.TransactionTagRepository.Insert
                        (
                        new TransactionTag
                        {
                            ClientName = dataSource.ClientName,
                            Tag = tag,
                            TransactionId = transactionId
                        }
                        );
                }
                unitOfWork.Save();
            }
            return new ActionResponse
            {
                Status = ActionResponseCode.Success
            };

        }
    }
}
